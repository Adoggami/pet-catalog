namespace PetCatalog.Application.DTOs;

public record PetDto(
    int Id,
    string Name,
    string Species,
    string? Breed,
    int? Age,
    string? Color,
    decimal? Weight,
    string? Description,
    string? ImageUrl,
    bool IsAvailable,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreatePetDto(
    string Name,
    string Species,
    string? Breed = null,
    int? Age = null,
    string? Color = null,
    decimal? Weight = null,
    string? Description = null,
    string? ImageUrl = null,
    bool IsAvailable = true
);

public record UpdatePetDto(
    string? Name = null,
    string? Species = null,
    string? Breed = null,
    int? Age = null,
    string? Color = null,
    decimal? Weight = null,
    string? Description = null,
    string? ImageUrl = null,
    bool? IsAvailable = null
);

public record PetListDto(
    IEnumerable<PetDto> Pets,
    int Count,
    int Limit,
    int Offset
);
