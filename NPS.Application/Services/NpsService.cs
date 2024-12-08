using Microsoft.Extensions.Configuration;
using NPS.Core.Entities;
using NPS.Infrastructure.Persistence;

namespace NPS.Application.Services;

// implementar a lógica de cálculo de NPS, categorização das respostas e outros cálculos.
public class NpsService
{
    private readonly IConfiguration _configuration;
    private readonly NpsDbContext _context;

    public NpsService(IConfiguration configuration, NpsDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public void SaveNps(Nps nps)
    {}
}
