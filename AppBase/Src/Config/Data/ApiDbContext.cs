using AppBase.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace AppBase.Config.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<LandMark> LandMarks { get; set; }
    public DbSet<Street> Streets { get; set; }
    public DbSet<Square> Squares { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LandMark>(entity =>
        {
            entity.Property(e => e.Version).IsRowVersion().IsConcurrencyToken();

            entity.Property(e => e.Geometry).HasColumnType("geometry(Point, 4326)");

            // Unique constraint for Name
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("Idx_LandMark_Name_Unique");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.Property(e => e.Version).IsRowVersion().IsConcurrencyToken();

            entity.Property(e => e.Geometry).HasColumnType("geometry(LineString, 4326)");

            // Unique constraint for Name
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("Idx_Street_Name_Unique");
        });

        modelBuilder.Entity<Square>(entity =>
        {
            entity.Property(e => e.Version).IsRowVersion().IsConcurrencyToken();

            entity.Property(e => e.Geometry).HasColumnType("geometry(Polygon, 4326)");

            // Unique constraint for Name
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("Idx_Polygon_Name_Unique");
        });

        base.OnModelCreating(modelBuilder);
    }
}