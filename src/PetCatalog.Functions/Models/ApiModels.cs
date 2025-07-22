using System.ComponentModel.DataAnnotations;

namespace PetCatalog.Functions.Models;

public class CreatePetRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Species { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Breed { get; set; }
    
    [Range(0, 50)]
    public int? Age { get; set; }
    
    [StringLength(50)]
    public string? Color { get; set; }
    
    [Range(0.1, 200.0)]
    public decimal? Weight { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [StringLength(255)]
    [Url]
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable { get; set; } = true;
}

public class UpdatePetRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }
    
    [StringLength(50, MinimumLength = 1)]
    public string? Species { get; set; }
    
    [StringLength(100)]
    public string? Breed { get; set; }
    
    [Range(0, 50)]
    public int? Age { get; set; }
    
    [StringLength(50)]
    public string? Color { get; set; }
    
    [Range(0.1, 200.0)]
    public decimal? Weight { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [StringLength(255)]
    [Url]
    public string? ImageUrl { get; set; }
    
    public bool? IsAvailable { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResult(string error, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            Message = message
        };
    }
}

public class ValidationErrorResponse
{
    public string Message { get; set; } = "Validation failed";
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
