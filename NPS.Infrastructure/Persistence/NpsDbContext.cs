using Microsoft.EntityFrameworkCore;
using NPS.Core.Entities;

namespace NPS.Infrastructure.Persistence;

public class NpsDbContext : DbContext
{
    public DbSet<Nps> Nps { get; set; }

    public NpsDbContext(DbContextOptions<NpsDbContext> options) : base(options)
    {
    }
}
