using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

public class SalesEventHandler : 
    INotificationHandler<SaleCreatedEvent>, 
    INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SalesEventHandler> _logger;

    public SalesEventHandler(ILogger<SalesEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(">>> [EVENTO] Venda {SaleNumber} criada em {Date}.", 
            notification.SaleNumber, notification.OccurredOn);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(">>> [EVENTO] Venda {SaleId} foi CANCELADA em {Date}.", 
            notification.SaleId, notification.OccurredOn);
        return Task.CompletedTask;
    }
}