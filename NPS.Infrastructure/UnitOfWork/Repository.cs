using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NPS.Infrastructure.Persistence;

namespace NPS.Infrastructure.UnitOfWork;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly NpsDbContext _context;

    public Repository(NpsDbContext context)
    {
        _context = context;
    }
    
    public async Task<T?> Get(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(expression);
    }

    public IEnumerable<T> GetAll()
    {
        return [.. _context.Set<T>().ToList()];
    }

    public async Task<T> Create(T command)
    {
        await _context.Set<T>().AddAsync(command);
        return command;
    }

    public Task<T> Update(T commandUpdate)
    {
        _context.Set<T>().Update(commandUpdate);
        return Task.FromResult(commandUpdate);
    }

    public Task Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRange(List<T> range)
    {
        _context.Set<T>().RemoveRange(range);
        return Task.CompletedTask;
    }

    public Task<int> Count()
    {
        return _context.Set<T>().CountAsync();
    }
}
