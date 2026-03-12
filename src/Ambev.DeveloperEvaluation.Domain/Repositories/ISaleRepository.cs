using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<(IEnumerable<Sale> Data, int TotalCount)> GetAllPaginatedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
    void RemoveItemsRange(IEnumerable<SaleItem> items);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
