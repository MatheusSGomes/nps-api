using System.Linq.Expressions;

namespace NPS.Infrastructure.UnitOfWork;

public interface IRepository<T>
{
    // Expression - Encapsulamento da expressão lambda
    // Func<TipoRecebido, TipoRetornado> - Expressão lambda, recebe generic e retorna boolean
    // Uso: Get(x => x.Id == id);
    Task<T?> Get(Expression<Func<T, bool>> expression);
    IEnumerable<T> GetAll();
    Task<T> Create(T command);
    Task<T> Update(T commandUpdate);
    Task Delete(T entity);
    Task DeleteRange(List<T> range);
    Task<int> Count();
}
