using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Handle should return mapped result when sale exists")]
    public async Task Handle_ExistingSale_ShouldReturnMappedResult()
    {
        var command = new GetSaleCommand(Guid.NewGuid());
        var saleEntity = new Sale("SALE-123", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        var expectedResult = new GetSaleResult { Id = command.Id, SaleNumber = "SALE-123" };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);
        _mapper.Map<GetSaleResult>(saleEntity).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.SaleNumber.Should().Be("SALE-123");
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Handle should throw KeyNotFoundException when sale does not exist")]
    public async Task Handle_NonExistingSale_ShouldThrowException()
    {
        var command = new GetSaleCommand(Guid.NewGuid());
        
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null); 

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} was not found.");
    }
}