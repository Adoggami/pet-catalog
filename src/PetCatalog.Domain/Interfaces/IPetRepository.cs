using PetCatalog.Domain.Entities;

namespace PetCatalog.Domain.Interfaces;

public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<Pet> CreateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<Pet?> UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
