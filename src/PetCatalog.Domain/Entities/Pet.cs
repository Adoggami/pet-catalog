using System.ComponentModel.DataAnnotations;

namespace PetCatalog.Domain.Entities;

public class Pet
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Species { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? Breed { get; set; }
    
    public int? Age { get; set; }
    
    [StringLength(50)]
    public string? Color { get; set; }
    
    public decimal? Weight { get; set; }
    
    public string? Description { get; set; }
    
    [StringLength(255)]
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
