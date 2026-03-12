namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    
    public CustomerResult Customer { get; set; } = null!;
    public BranchResult Branch { get; set; } = null!;
    public List<SaleItemResult> Items { get; set; } = new();
}

public record CustomerResult(Guid Id, string Name);
public record BranchResult(Guid Id, string Name);
public record SaleItemResult(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Discount, decimal TotalAmount, bool IsCancelled);