using NPS.Core.Interfaces.Repositorios;

namespace NPS.Infrastructure.UnitOfWork;

public interface IUnitOfWork
{
    public INpsRepository NpsRepository { get; }
    void Commit();
}
