using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PetCatalog.Application.DTOs;
using PetCatalog.Application.Services;
using PetCatalog.Domain.Entities;
using PetCatalog.Domain.Interfaces;
using Xunit;

namespace PetCatalog.UnitTests.Application.Services;

public class PetApplicationServiceTests
{
    private readonly Mock<IPetRepository> _petRepositoryMock;
    private readonly Mock<ILogger<PetApplicationService>> _loggerMock;
    private readonly PetApplicationService _service;

    public PetApplicationServiceTests()
    {
        _petRepositoryMock = new Mock<IPetRepository>();
        _loggerMock = new Mock<ILogger<PetApplicationService>>();
        _service = new PetApplicationService(_petRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPetByIdAsync_WhenPetExists_ShouldReturnPetDto()
    {
        // Arrange
        var petId = 1;
        var pet = new Pet
        {
            Id = petId,
            Name = "Buddy",
            Species = "Dog",
            Breed = "Golden Retriever",
            Age = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _petRepositoryMock
            .Setup(x => x.GetByIdAsync(petId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pet);

        // Act
        var result = await _service.GetPetByIdAsync(petId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(petId);
        result.Name.Should().Be("Buddy");
        result.Species.Should().Be("Dog");
        result.Breed.Should().Be("Golden Retriever");
        result.Age.Should().Be(3);
    }

    [Fact]
    public async Task GetPetByIdAsync_WhenPetDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var petId = 999;

        _petRepositoryMock
            .Setup(x => x.GetByIdAsync(petId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pet?)null);

        // Act
        var result = await _service.GetPetByIdAsync(petId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePetAsync_WithValidData_ShouldReturnCreatedPetDto()
    {
        // Arrange
        var createDto = new CreatePetDto(
            "Max",
            "Dog",
            "German Shepherd",
            5,
            "Brown",
            35.0m,
            "Intelligent and loyal dog",
            "https://example.com/image.jpg",
            true
        );

        var createdPet = new Pet
        {
            Id = 1,
            Name = createDto.Name,
            Species = createDto.Species,
            Breed = createDto.Breed,
            Age = createDto.Age,
            Color = createDto.Color,
            Weight = createDto.Weight,
            Description = createDto.Description,
            ImageUrl = createDto.ImageUrl,
            IsAvailable = createDto.IsAvailable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _petRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdPet);

        // Act
        var result = await _service.CreatePetAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Max");
        result.Species.Should().Be("Dog");
        result.Breed.Should().Be("German Shepherd");
        result.Age.Should().Be(5);
    }

    [Fact]
    public async Task DeletePetAsync_WhenPetExists_ShouldReturnTrue()
    {
        // Arrange
        var petId = 1;

        _petRepositoryMock
            .Setup(x => x.DeleteAsync(petId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeletePetAsync(petId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePetAsync_WhenPetDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var petId = 999;

        _petRepositoryMock
            .Setup(x => x.DeleteAsync(petId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeletePetAsync(petId);

        // Assert
        result.Should().BeFalse();
    }
}
