using KineticTechnicalChallenge.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KineticTechnicalChallenge.Core.Data
{
    public class DocumentContext : DbContext
    {
        public DocumentContext(DbContextOptions<DocumentContext> options)
            : base(options)
        {
        }

        public DbSet<ProcessInfo> Processes { get; set; }
        public DbSet<AnalysisResult> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessInfo>();
        }
    }
}
