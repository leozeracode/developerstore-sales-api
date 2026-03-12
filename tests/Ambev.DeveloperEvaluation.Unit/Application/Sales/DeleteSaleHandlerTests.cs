using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Handle should cancel sale, update repository and publish event when sale exists")]
    public async Task Handle_ExistingSale_ShouldCancelUpdateAndPublishEvent()
    {
        var command = new DeleteSaleCommand(Guid.NewGuid());
        var saleEntity = new Sale("SALE-123", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        saleEntity.IsCancelled.Should().BeTrue();

        await _saleRepository.Received(1).UpdateAsync(saleEntity, Arg.Any<CancellationToken>());

        await _mediator.Received(1).Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Handle should return false when sale does not exist")]
    public async Task Handle_NonExistingSale_ShouldReturnFalse()
    {
        var command = new DeleteSaleCommand(Guid.NewGuid());

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();

        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.DidNotReceive().Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }
}