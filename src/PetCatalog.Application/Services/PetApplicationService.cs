using Microsoft.Extensions.Logging;
using PetCatalog.Application.DTOs;
using PetCatalog.Application.Interfaces;
using PetCatalog.Domain.Entities;
using PetCatalog.Domain.Interfaces;

namespace PetCatalog.Application.Services;

public class PetApplicationService : IPetApplicationService
{
    private readonly IPetRepository _petRepository;
    private readonly ILogger<PetApplicationService> _logger;

    public PetApplicationService(IPetRepository petRepository, ILogger<PetApplicationService> logger)
    {
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PetDto?> GetPetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting pet with ID: {PetId}", id);
        
        var pet = await _petRepository.GetByIdAsync(id, cancellationToken);
        return pet != null ? MapToDto(pet) : null;
    }

    public async Task<PetListDto> GetPetsAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting pets with limit: {Limit}, offset: {Offset}", limit, offset);
        
        var pets = await _petRepository.GetAllAsync(limit, offset, cancellationToken);
        var petDtos = pets.Select(MapToDto).ToList();
        
        return new PetListDto(petDtos, petDtos.Count, limit, offset);
    }

    public async Task<PetDto> CreatePetAsync(CreatePetDto createPetDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new pet: {PetName}", createPetDto.Name);
        
        var pet = new Pet
        {
            Name = createPetDto.Name,
            Species = createPetDto.Species,
            Breed = createPetDto.Breed,
            Age = createPetDto.Age,
            Color = createPetDto.Color,
            Weight = createPetDto.Weight,
            Description = createPetDto.Description,
            ImageUrl = createPetDto.ImageUrl,
            IsAvailable = createPetDto.IsAvailable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPet = await _petRepository.CreateAsync(pet, cancellationToken);
        return MapToDto(createdPet);
    }

    public async Task<PetDto?> UpdatePetAsync(int id, UpdatePetDto updatePetDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating pet with ID: {PetId}", id);
        
        var existingPet = await _petRepository.GetByIdAsync(id, cancellationToken);
        if (existingPet == null)
        {
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(updatePetDto.Name))
            existingPet.Name = updatePetDto.Name;
        
        if (!string.IsNullOrWhiteSpace(updatePetDto.Species))
            existingPet.Species = updatePetDto.Species;
        
        if (updatePetDto.Breed != null)
            existingPet.Breed = updatePetDto.Breed;
        
        if (updatePetDto.Age.HasValue)
            existingPet.Age = updatePetDto.Age;
        
        if (updatePetDto.Color != null)
            existingPet.Color = updatePetDto.Color;
        
        if (updatePetDto.Weight.HasValue)
            existingPet.Weight = updatePetDto.Weight;
        
        if (updatePetDto.Description != null)
            existingPet.Description = updatePetDto.Description;
        
        if (updatePetDto.ImageUrl != null)
            existingPet.ImageUrl = updatePetDto.ImageUrl;
        
        if (updatePetDto.IsAvailable.HasValue)
            existingPet.IsAvailable = updatePetDto.IsAvailable.Value;

        existingPet.UpdatedAt = DateTime.UtcNow;

        var updatedPet = await _petRepository.UpdateAsync(existingPet, cancellationToken);
        return updatedPet != null ? MapToDto(updatedPet) : null;
    }

    public async Task<bool> DeletePetAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting pet with ID: {PetId}", id);
        
        return await _petRepository.DeleteAsync(id, cancellationToken);
    }

    private static PetDto MapToDto(Pet pet)
    {
        return new PetDto(
            pet.Id,
            pet.Name,
            pet.Species,
            pet.Breed,
            pet.Age,
            pet.Color,
            pet.Weight,
            pet.Description,
            pet.ImageUrl,
            pet.IsAvailable,
            pet.CreatedAt,
            pet.UpdatedAt
        );
    }
}
