using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, string SaleNumber, DateTime OccurredOn) : INotification;
public record SaleModifiedEvent(Guid SaleId, DateTime OccurredOn) : INotification;
public record SaleCancelledEvent(Guid SaleId, DateTime OccurredOn) : INotification;
public record ItemCancelledEvent(Guid SaleId, Guid ProductId, DateTime OccurredOn);