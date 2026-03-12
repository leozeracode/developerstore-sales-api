using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Theory(DisplayName = "Should calculate discount correctly based on quantity tiers")]
    [InlineData(1, 100, 0)]
    [InlineData(3, 100, 0)]
    [InlineData(4, 100, 40)]
    [InlineData(9, 100, 90)]
    [InlineData(10, 100, 200)]
    [InlineData(20, 100, 400)]
    public void CreateSaleItem_WithValidQuantities_ShouldCalculateDiscountCorrectly(
        int quantity, decimal unitPrice, decimal expectedDiscount)
    {
        var productId = Guid.NewGuid();
        var productName = "Test Product";

        var item = new SaleItem(productId, productName, quantity, unitPrice);

        Assert.Equal(expectedDiscount, item.Discount);

        var expectedTotal = (quantity * unitPrice) - expectedDiscount;
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when quantity is greater than 20")]
    public void CreateSaleItem_WithQuantityGreaterThan20_ShouldThrowException()
    {
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var invalidQuantity = 21;
        var unitPrice = 100m;

        var exception = Record.Exception(() => new SaleItem(productId, productName, invalidQuantity, unitPrice));

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("Cannot sell more than 20 identical items.", exception.Message);
    }

    [Theory(DisplayName = "Should throw ArgumentException when quantity is zero or negative")]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateSaleItem_WithZeroOrNegativeQuantity_ShouldThrowException(int invalidQuantity)
    {
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var unitPrice = 100m;

        var exception = Record.Exception(() => new SaleItem(productId, productName, invalidQuantity, unitPrice));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Quantity must be greater than zero.", exception.Message);
    }
}