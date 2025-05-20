using KR2.FileAnalysisService.Models;
using Microsoft.EntityFrameworkCore;

namespace KR2.FileAnalysisService.Data;

public class FileAnalysisDbContext : DbContext
{
    public FileAnalysisDbContext(DbContextOptions<FileAnalysisDbContext> options) : base(options)
    {
    }

    public DbSet<FileAnalysisResult> AnalysisResults { get; set; } = null!;
    public DbSet<FileSimilarityResult> SimilarityResults { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileAnalysisResult>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<FileAnalysisResult>()
            .HasIndex(r => r.FileId)
            .IsUnique();

        modelBuilder.Entity<FileSimilarityResult>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<FileSimilarityResult>()
            .HasOne(s => s.FileAnalysisResult)
            .WithMany(a => a.SimilarityResults)
            .HasForeignKey(s => s.FileAnalysisResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
