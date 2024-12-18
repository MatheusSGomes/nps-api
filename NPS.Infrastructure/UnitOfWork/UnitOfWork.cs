using NPS.Core.Interfaces.Repositorios;
using NPS.Infrastructure.Persistence;
using NPS.Infrastructure.Repositories;

namespace NPS.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly NpsDbContext _context;
    private readonly INpsRepository _npsRepository;

    public UnitOfWork(NpsDbContext context, INpsRepository npsRepository)
    {
        _context = context;
        _npsRepository = npsRepository;
    }

    public INpsRepository NpsRepository => _npsRepository ?? new NpsRepository(_context);

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}
