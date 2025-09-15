using MetaExtractor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaExtractor.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Face> Faces { get; set; }
    public DbSet<Metadata> Metadata { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
