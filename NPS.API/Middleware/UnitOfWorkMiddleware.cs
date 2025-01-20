using NPS.Infrastructure.UnitOfWork;

namespace NPS.API.Middleware;

public class UnitOfWorkMiddleware
{
    public RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Após invocar toda a aplicação
        await _next(context);

        // Recupero o UnitOfWork para aplicar o commit (SaveChanges)
        var unitOfWork = (IUnitOfWork)context.RequestServices.GetService(typeof(IUnitOfWork));

        if (unitOfWork != null)
            await unitOfWork.Commit();
    }
}
