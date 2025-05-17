using KR2.FileStoringService.Models;
using Microsoft.EntityFrameworkCore;

namespace KR2.FileStoringService.Data;

public class FileStoringDbContext(DbContextOptions<FileStoringDbContext> options) : DbContext(options)
{
    public DbSet<FileRecord> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileRecord>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<FileRecord>()
            .HasIndex(f => f.FileHash)
            .IsUnique();
    }
}
