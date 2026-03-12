using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM.Repositories;

public class SaleRepositoryTests
{
    private DefaultContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new DefaultContext(options);
    }

    [Fact(DisplayName = "CreateAsync should persist sale and generate an ID")]
    public async Task CreateAsync_ValidSale_ShouldPersistToDatabase()
    {
        await using var context = CreateDbContext();
        var repository = new SaleRepository(context);
        var sale = new Sale("SALE-001", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");

        var result = await repository.CreateAsync(sale);

        result.Should().NotBeNull();

        var savedSale = await context.Sales.FirstOrDefaultAsync(s => s.SaleNumber == "SALE-001");
        savedSale.Should().NotBeNull();
        savedSale.Customer.Name.Should().Be("Customer");
    }

    [Fact(DisplayName = "GetByIdAsync should return sale with items included")]
    public async Task GetByIdAsync_ExistingSale_ShouldReturnWithItems()
    {
        await using var context = CreateDbContext();
        var repository = new SaleRepository(context);

        var sale = new Sale("SALE-002", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 2, 50m);

        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(sale.Id);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1); 
        result.Items.First().ProductName.Should().Be("Product A");
    }

    [Fact(DisplayName = "GetAllPaginatedAsync should return correct page and total count")]
    public async Task GetAllPaginatedAsync_MultipleSales_ShouldReturnCorrectPage()
    {
        await using var context = CreateDbContext();
        var repository = new SaleRepository(context);

        for (int i = 1; i <= 5; i++)
        {
            var sale = new Sale($"SALE-00{i}", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
            await context.Sales.AddAsync(sale);
        }

        await context.SaveChangesAsync();

        var (data, totalCount) = await repository.GetAllPaginatedAsync(page: 2, size: 2);

        totalCount.Should().Be(5);
        data.Should().HaveCount(2);

        data.Should().OnlyContain(s => s.SaleNumber.StartsWith("SALE-00"));
    }
    
    [Fact(DisplayName = "UpdateAsync should modify existing sale and persist changes")]
    public async Task UpdateAsync_ExistingSale_ShouldModifyDatabaseRecord()
    {
        await using var context = CreateDbContext();
        var repository = new SaleRepository(context);
        
        var sale = new Sale("SALE-ORIGINAL", Guid.NewGuid(), "Old Customer", Guid.NewGuid(), "Old Branch");
        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        sale.UpdateBasicInfo(
            "SALE-UPDATED", 
            DateTime.UtcNow, 
            new Customer(sale.Customer.Id, "New Customer"), 
            new Branch(sale.Branch.Id, "New Branch")
        );

        await repository.UpdateAsync(sale);

        var updatedSale = await context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        
        updatedSale.Should().NotBeNull();
        updatedSale!.SaleNumber.Should().Be("SALE-UPDATED");
        updatedSale.Customer.Name.Should().Be("New Customer");
        updatedSale.Branch.Name.Should().Be("New Branch");
    }

    [Fact(DisplayName = "DeleteAsync should physically remove the sale from the database")]
    public async Task DeleteAsync_ExistingSale_ShouldRemoveFromDatabase()
    {
        await using var context = CreateDbContext();
        var repository = new SaleRepository(context);
        
        var sale = new Sale("SALE-TO-DELETE", Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();

        var result = await repository.DeleteAsync(sale.Id);

        result.Should().BeTrue();
        
        var deletedSale = await context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        deletedSale.Should().BeNull();
    }
}