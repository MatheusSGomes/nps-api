using NPS.Core.Entities;
using NPS.Core.Interfaces.Repositorios;
using NPS.Infrastructure.Persistence;

namespace NPS.Infrastructure.Repositories;

public class NpsRepository : INpsRepository
{
    private readonly NpsDbContext _context;

    public NpsRepository(NpsDbContext context)
    {
        _context = context;
    }

    public void Create(Nps nps)
    {
        _context.Set<Nps>().Add(nps);
    }
}
