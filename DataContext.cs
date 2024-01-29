using Microsoft.EntityFrameworkCore;

namespace WebScraper.Data
{
  public class DataContext : DbContext
  {
    public DbSet<ScraperData> Scraps { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ScraperData>().HasKey(s => s.Id);
      modelBuilder.Entity<ScraperData>().Property(s => s.Id).ValueGeneratedOnAdd();
      base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
  }
}