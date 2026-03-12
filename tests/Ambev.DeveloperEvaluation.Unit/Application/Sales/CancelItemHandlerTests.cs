using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelItemHandler _handler;

    public CancelItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelItemHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Handle should cancel specific item, update repository and publish ItemCancelled event")]
    public async Task Handle_ValidItem_ShouldCancelAndPublishEvent()
    {
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var saleEntity = new Sale("SALE-123", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        
        saleEntity.AddItem(productId, "Product A", 1, 10m);
        var itemId = saleEntity.Items.First().Id;

        var command = new CancelItemCommand(saleId, itemId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(saleEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        saleEntity.Items.First().IsCancelled.Should().BeTrue(); 

        await _saleRepository.Received(1).UpdateAsync(saleEntity, Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }
}