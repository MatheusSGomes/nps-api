using Microsoft.EntityFrameworkCore;
using NPS.Core.Entities;
using NPS.Infrastructure.Persistence;
using NPS.Infrastructure.Repositories;
using NPS.Infrastructure.UnitOfWork;

namespace NPS.Test.Integration;

public class NpsRepositoryTests
{
    private readonly NpsDbContext _context;
    private readonly NpsRepository _repository;
    private readonly UnitOfWork _unitOfWork;

    public NpsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<NpsDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _context = new NpsDbContext(options);
        _repository = new NpsRepository(_context);
        _unitOfWork = new UnitOfWork(_context, _repository);
    }

    [Fact]
    public async Task SaveUserNps_DeveSalvarNpsCorreto()
    {
        // Arrange
        var expectedScore = 10;
        var expectedCustomerName = "Cliente Teste";
        var expectedComment = "Comentário Teste";
        var nps = Nps.CreateResponse(expectedScore, expectedCustomerName, expectedComment);

        // Act
        _unitOfWork.NpsRepository.Create(nps);
        await _unitOfWork.Commit();

        // Assert
        var savedNps = await _context.Nps.FirstOrDefaultAsync(x => x.CustomerName == expectedCustomerName);
        Assert.NotNull(savedNps);
        Assert.Equal(expectedScore, savedNps.Score);
        Assert.Equal(expectedCustomerName, savedNps.CustomerName);
        Assert.Equal(expectedComment, savedNps.Comment);
    }
}
