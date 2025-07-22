using PetCatalog.Application.DTOs;
using PetCatalog.Functions.Models;

namespace PetCatalog.Functions.Services.Interfaces;

public interface IPetService
{
    Task<ApiResponse<PetDto?>> GetPetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PetListDto>> GetPetsAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<ApiResponse<PetDto>> CreatePetAsync(CreatePetRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<PetDto?>> UpdatePetAsync(int id, UpdatePetRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeletePetAsync(int id, CancellationToken cancellationToken = default);
    ValidationErrorResponse ValidateCreateRequest(CreatePetRequest request);
    ValidationErrorResponse ValidateUpdateRequest(UpdatePetRequest request);
}
