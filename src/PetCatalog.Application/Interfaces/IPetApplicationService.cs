using PetCatalog.Application.DTOs;

namespace PetCatalog.Application.Interfaces;

public interface IPetApplicationService
{
    Task<PetDto?> GetPetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PetListDto> GetPetsAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<PetDto> CreatePetAsync(CreatePetDto createPetDto, CancellationToken cancellationToken = default);
    Task<PetDto?> UpdatePetAsync(int id, UpdatePetDto updatePetDto, CancellationToken cancellationToken = default);
    Task<bool> DeletePetAsync(int id, CancellationToken cancellationToken = default);
}
