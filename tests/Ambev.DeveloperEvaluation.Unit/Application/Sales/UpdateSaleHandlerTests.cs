using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new UpdateSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "Handle should update sale, recalculate items and publish Modified event")]
    public async Task Handle_ValidUpdate_ShouldUpdateAndPublishEvent()
    {
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand { Id = saleId, SaleNumber = "SALE-UPDATED" };
        var saleEntity = new Sale("SALE-OLD", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.SaleNumber.Should().Be("SALE-UPDATED");
        saleEntity.SaleNumber.Should().Be("SALE-UPDATED");
        
        await _saleRepository.Received(1).UpdateAsync(saleEntity, Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Handle should throw exception when updating a cancelled sale")]
    public async Task Handle_CancelledSale_ShouldThrowException()
    {
        var command = new UpdateSaleCommand { Id = Guid.NewGuid() };
        var saleEntity = new Sale("SALE-123", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        saleEntity.CancelSale(); 

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(saleEntity);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale.");
    }
}