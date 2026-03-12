using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommand : IRequest<bool>
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }

    public CancelItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}