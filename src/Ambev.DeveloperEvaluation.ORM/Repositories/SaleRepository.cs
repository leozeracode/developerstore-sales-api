using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }
    
    public async Task<(IEnumerable<Sale> Data, int TotalCount)> GetAllPaginatedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var tracked = _context.Sales.Local.FirstOrDefault(s => s.Id == sale.Id);
        if (tracked != null)
            _context.Entry(tracked).State = EntityState.Detached;

        _context.Sales.Attach(sale);
        _context.Entry(sale).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    
        return sale;
    }

    public void RemoveItemsRange(IEnumerable<SaleItem> items)
    {
        _context.Set<SaleItem>().RemoveRange(items);
    }
    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}