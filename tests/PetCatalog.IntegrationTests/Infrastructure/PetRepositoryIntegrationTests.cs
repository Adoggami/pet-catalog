using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetCatalog.Domain.Entities;
using PetCatalog.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace PetCatalog.IntegrationTests.Infrastructure;

public class PetRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    private IServiceProvider _serviceProvider = null!;
    private PetCatalogDbContext _context = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();
        
        services.AddLogging(builder => builder.AddConsole());
        
        services.AddDbContext<PetCatalogDbContext>(options =>
        {
            options.UseNpgsql(_postgres.GetConnectionString());
        });

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<PetCatalogDbContext>();

        // Ensure database is created
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _serviceProvider.GetRequiredService<IServiceScope>().DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreatePet_ShouldPersistToDatabase()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "Integration Test Pet",
            Species = "Dog",
            Breed = "Test Breed",
            Age = 2,
            Description = "Test pet for integration testing",
            IsAvailable = true
        };

        // Act
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        // Assert
        var savedPet = await _context.Pets.FirstOrDefaultAsync(p => p.Name == "Integration Test Pet");
        savedPet.Should().NotBeNull();
        savedPet!.Species.Should().Be("Dog");
        savedPet.Breed.Should().Be("Test Breed");
        savedPet.Age.Should().Be(2);
    }

    [Fact]
    public async Task GetPetById_WhenPetExists_ShouldReturnPet()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "Find Me",
            Species = "Cat",
            Age = 1,
            IsAvailable = true
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var foundPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == pet.Id);

        // Assert
        foundPet.Should().NotBeNull();
        foundPet!.Name.Should().Be("Find Me");
        foundPet.Species.Should().Be("Cat");
        foundPet.Age.Should().Be(1);
    }

    [Fact]
    public async Task UpdatePet_ShouldModifyExistingPet()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "Original Name",
            Species = "Dog",
            Age = 3,
            IsAvailable = true
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        pet.Name = "Updated Name";
        pet.Age = 4;
        await _context.SaveChangesAsync();

        // Assert
        var updatedPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == pet.Id);
        updatedPet.Should().NotBeNull();
        updatedPet!.Name.Should().Be("Updated Name");
        updatedPet.Age.Should().Be(4);
    }

    [Fact]
    public async Task DeletePet_ShouldRemoveFromDatabase()
    {
        // Arrange
        var pet = new Pet
        {
            Name = "To Be Deleted",
            Species = "Bird",
            IsAvailable = true
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        var petId = pet.Id;

        // Act
        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        // Assert
        var deletedPet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == petId);
        deletedPet.Should().BeNull();
    }
}
