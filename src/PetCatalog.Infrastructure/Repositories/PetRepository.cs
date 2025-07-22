using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetCatalog.Domain.Entities;
using PetCatalog.Domain.Interfaces;
using PetCatalog.Infrastructure.Data;

namespace PetCatalog.Infrastructure.Repositories;

public class PetRepository : IPetRepository
{
    private readonly PetCatalogDbContext _context;
    private readonly ILogger<PetRepository> _logger;

    public PetRepository(PetCatalogDbContext context, ILogger<PetRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting pet by ID: {PetId}", id);
        
        return await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Pet>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting pets with limit: {Limit}, offset: {Offset}", limit, offset);
        
        return await _context.Pets
            .OrderBy(p => p.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pet> CreateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating pet: {PetName}", pet.Name);
        
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync(cancellationToken);
        
        return pet;
    }

    public async Task<Pet?> UpdateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating pet: {PetId}", pet.Id);
        
        var existingPet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == pet.Id, cancellationToken);
        
        if (existingPet == null)
        {
            return null;
        }

        // Update properties
        _context.Entry(existingPet).CurrentValues.SetValues(pet);
        existingPet.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        return existingPet;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting pet: {PetId}", id);
        
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        
        if (pet == null)
        {
            return false;
        }

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pets
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}
