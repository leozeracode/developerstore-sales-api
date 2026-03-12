using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public Branch Branch { get; private set; } = null!;

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    public bool IsCancelled { get; private set; }

    public Sale()
    {
    }

    public Sale(string saleNumber, DateTime saleDate, Customer customer, Branch branch)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        Customer = customer;
        Branch = branch;
        IsCancelled = false;
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > 20)
                throw new InvalidOperationException(
                    "The total quantity of this product exceeds the limit of 20 items.");

            _items.Remove(existingItem);
            _items.Add(new SaleItem(productId, productName, newQuantity, unitPrice));
        }
        else
        {
            _items.Add(new SaleItem(productId, productName, quantity, unitPrice));
        }
    }

    public void CancelItem(Guid productId)
    {
        if (IsCancelled) return;

        var item = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        item?.Cancel();
    }

    public void CancelSale()
    {
        if (IsCancelled) return;

        IsCancelled = true;
        foreach (var item in _items)
        {
            item.Cancel();
        }
    }
}