using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();

        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Handle should process valid command, save to repository and publish event")]
    public async Task Handle_ValidCommand_ShouldCreateSaleAndPublishEvent()
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-2026",
            SaleDate = DateTime.UtcNow, 
            CustomerId = Guid.NewGuid(),
            CustomerName = "Leonardo",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Principal",
            Items = new List<CreateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Produto A", Quantity = 5, UnitPrice = 100m }
            }
        };

        var saleId = Guid.NewGuid();
        var saleEntity = new Sale(command.SaleNumber, command.CustomerId, command.CustomerName, command.BranchId,
            command.BranchName);
        var expectedResult = new CreateSaleResult { Id = saleId, SaleNumber = command.SaleNumber };

        _mapper.Map<Sale>(command).Returns(saleEntity);
        _mapper.Map<CreateSaleResult>(saleEntity).Returns(expectedResult);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(saleEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.SaleNumber.Should().Be(command.SaleNumber);

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}