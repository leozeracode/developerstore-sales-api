using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} was not found.");

        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        if (sale.Items.Any())
        {
            _saleRepository.RemoveItemsRange(sale.Items.ToList());
            sale.ClearItems();
        }

        sale.UpdateBasicInfo(
            command.SaleNumber, 
            command.SaleDate, 
            new Customer(command.CustomerId, command.CustomerName), 
            new Branch(command.BranchId, command.BranchName)
        );

        foreach (var item in command.Items)
        {
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new UpdateSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };
    }
}