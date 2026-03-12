using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemHandler : IRequestHandler<CancelItemCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    public CancelItemHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(CancelItemCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        
        if (sale == null) return false;

        sale.CancelItem(command.ItemId);
        
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mediator.Publish(new ItemCancelledEvent(sale.Id, command.ItemId, DateTime.UtcNow), cancellationToken);

        return true;
    }
}