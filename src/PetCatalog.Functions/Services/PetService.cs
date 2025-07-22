using Microsoft.Extensions.Logging;
using PetCatalog.Application.DTOs;
using PetCatalog.Application.Interfaces;
using PetCatalog.Functions.Models;
using PetCatalog.Functions.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PetCatalog.Functions.Services;

public class PetService : IPetService
{
    private readonly IPetApplicationService _petApplicationService;
    private readonly ILogger<PetService> _logger;

    public PetService(IPetApplicationService petApplicationService, ILogger<PetService> logger)
    {
        _petApplicationService = petApplicationService;
        _logger = logger;
    }

    public async Task<ApiResponse<PetDto?>> GetPetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<PetDto?>.ErrorResult("Invalid pet ID. Must be a positive integer.");
            }

            var pet = await _petApplicationService.GetPetByIdAsync(id, cancellationToken);
            
            if (pet == null)
            {
                return ApiResponse<PetDto?>.ErrorResult($"Pet with ID {id} not found.");
            }

            return ApiResponse<PetDto?>.SuccessResult(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pet with ID {PetId}", id);
            return ApiResponse<PetDto?>.ErrorResult("An error occurred while retrieving the pet.");
        }
    }

    public async Task<ApiResponse<PetListDto>> GetPetsAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit <= 0 || limit > 1000)
            {
                return ApiResponse<PetListDto>.ErrorResult("Invalid limit. Must be between 1 and 1000.");
            }

            if (offset < 0)
            {
                return ApiResponse<PetListDto>.ErrorResult("Invalid offset. Must be 0 or greater.");
            }

            var pets = await _petApplicationService.GetPetsAsync(limit, offset, cancellationToken);
            return ApiResponse<PetListDto>.SuccessResult(pets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pets with limit {Limit}, offset {Offset}", limit, offset);
            return ApiResponse<PetListDto>.ErrorResult("An error occurred while retrieving pets.");
        }
    }

    public async Task<ApiResponse<PetDto>> CreatePetAsync(CreatePetRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = ValidateCreateRequest(request);
            if (validationResult.Errors.Any())
            {
                return ApiResponse<PetDto>.ErrorResult("Validation failed", validationResult.Message);
            }

            var createDto = new CreatePetDto(
                request.Name,
                request.Species,
                request.Breed,
                request.Age,
                request.Color,
                request.Weight,
                request.Description,
                request.ImageUrl,
                request.IsAvailable
            );

            var createdPet = await _petApplicationService.CreatePetAsync(createDto, cancellationToken);
            return ApiResponse<PetDto>.SuccessResult(createdPet, "Pet created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pet {PetName}", request.Name);
            return ApiResponse<PetDto>.ErrorResult("An error occurred while creating the pet.");
        }
    }

    public async Task<ApiResponse<PetDto?>> UpdatePetAsync(int id, UpdatePetRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<PetDto?>.ErrorResult("Invalid pet ID. Must be a positive integer.");
            }

            var validationResult = ValidateUpdateRequest(request);
            if (validationResult.Errors.Any())
            {
                return ApiResponse<PetDto?>.ErrorResult("Validation failed", validationResult.Message);
            }

            var updateDto = new UpdatePetDto(
                request.Name,
                request.Species,
                request.Breed,
                request.Age,
                request.Color,
                request.Weight,
                request.Description,
                request.ImageUrl,
                request.IsAvailable
            );

            var updatedPet = await _petApplicationService.UpdatePetAsync(id, updateDto, cancellationToken);
            
            if (updatedPet == null)
            {
                return ApiResponse<PetDto?>.ErrorResult($"Pet with ID {id} not found.");
            }

            return ApiResponse<PetDto?>.SuccessResult(updatedPet, "Pet updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pet with ID {PetId}", id);
            return ApiResponse<PetDto?>.ErrorResult("An error occurred while updating the pet.");
        }
    }

    public async Task<ApiResponse<bool>> DeletePetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<bool>.ErrorResult("Invalid pet ID. Must be a positive integer.");
            }

            var deleted = await _petApplicationService.DeletePetAsync(id, cancellationToken);
            
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResult($"Pet with ID {id} not found.");
            }

            return ApiResponse<bool>.SuccessResult(true, "Pet deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pet with ID {PetId}", id);
            return ApiResponse<bool>.ErrorResult("An error occurred while deleting the pet.");
        }
    }

    public ValidationErrorResponse ValidateCreateRequest(CreatePetRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        var context = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(request, context, validationResults, true))
        {
            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames.Any())
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        if (!errors.ContainsKey(memberName))
                        {
                            errors[memberName] = new[] { validationResult.ErrorMessage ?? "Invalid value" };
                        }
                    }
                }
            }
        }

        return new ValidationErrorResponse { Errors = errors };
    }

    public ValidationErrorResponse ValidateUpdateRequest(UpdatePetRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        var context = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(request, context, validationResults, true))
        {
            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames.Any())
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        if (!errors.ContainsKey(memberName))
                        {
                            errors[memberName] = new[] { validationResult.ErrorMessage ?? "Invalid value" };
                        }
                    }
                }
            }
        }

        return new ValidationErrorResponse { Errors = errors };
    }
}
