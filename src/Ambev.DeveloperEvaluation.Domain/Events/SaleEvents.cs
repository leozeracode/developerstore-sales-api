namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, string SaleNumber, DateTime OccurredOn);
public record SaleModifiedEvent(Guid SaleId, DateTime OccurredOn);
public record SaleCancelledEvent(Guid SaleId, DateTime OccurredOn);
public record ItemCancelledEvent(Guid SaleId, Guid ProductId, DateTime OccurredOn);