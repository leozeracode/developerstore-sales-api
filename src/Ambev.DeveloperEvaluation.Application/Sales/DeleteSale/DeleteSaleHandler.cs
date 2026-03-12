using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;


    public DeleteSaleHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null) return false;

        sale.CancelSale();
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleCancelledEvent(
            sale.Id, 
            DateTime.UtcNow
        ), cancellationToken);

        return true;
    }
}