using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    public SaleItem()
    {
    }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new InvalidOperationException("Cannot sell more than 20 identical items.");

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsCancelled = false;

        CalculateDiscountAndTotal();
    }

    private void CalculateDiscountAndTotal()
    {
        decimal discountPercentage = 0m;

        if (Quantity >= 4 && Quantity < 10)
            discountPercentage = 0.10m;
        else if (Quantity >= 10 && Quantity <= 20)
            discountPercentage = 0.20m;

        var rawTotal = Quantity * UnitPrice;
        Discount = rawTotal * discountPercentage;
        TotalAmount = rawTotal - Discount;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}