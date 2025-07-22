using Microsoft.EntityFrameworkCore;
using PetCatalog.Domain.Entities;

namespace PetCatalog.Infrastructure.Data;

public class PetCatalogDbContext : DbContext
{
    public PetCatalogDbContext(DbContextOptions<PetCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Pet> Pets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Pet entity
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("pets");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");
            
            entity.Property(e => e.Species)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("species");
            
            entity.Property(e => e.Breed)
                .HasMaxLength(100)
                .HasColumnName("breed");
            
            entity.Property(e => e.Age)
                .HasColumnName("age");
            
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5,2)")
                .HasColumnName("weight");
            
            entity.Property(e => e.Description)
                .HasColumnName("description");
            
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            // Indexes
            entity.HasIndex(e => e.Species).HasDatabaseName("idx_pets_species");
            entity.HasIndex(e => e.IsAvailable).HasDatabaseName("idx_pets_available");
        });
    }
}
