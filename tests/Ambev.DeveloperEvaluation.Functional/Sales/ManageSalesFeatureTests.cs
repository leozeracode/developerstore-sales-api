using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public class ManageSalesFeatureTests
    : IClassFixture<WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>>
{
    private readonly HttpClient _client;

    public ManageSalesFeatureTests(
        WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Guid> CreateTestSaleAsync()
    {
        var request = new
        {
            saleNumber = $"TEST-{Guid.NewGuid().ToString()[..8]}",
            saleDate = DateTime.UtcNow,
            customerId = Guid.NewGuid(),
            customerName = "Leonardo Test",
            branchId = Guid.NewGuid(),
            branchName = "Branch Test",
            items = new[]
            {
                new
                {
                    productId = Guid.NewGuid(),
                    productName = "Produto Teste",
                    quantity = 2,
                    unitPrice = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/sales", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json
            .GetProperty("data")
            .GetProperty("id")
            .GetGuid();
    }

    [Fact]
    public async Task CreateSale_ShouldReturn201()
    {
        var request = new
        {
            saleNumber = $"TEST-{Guid.NewGuid().ToString()[..8]}",
            saleDate = DateTime.UtcNow,
            customerId = Guid.NewGuid(),
            customerName = "Leonardo Test",
            branchId = Guid.NewGuid(),
            branchName = "Branch Test",
            items = new[]
            {
                new
                {
                    productId = Guid.NewGuid(),
                    productName = "Produto Teste",
                    quantity = 2,
                    unitPrice = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/sales", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("success").GetBoolean().Should().BeTrue();
        json.GetProperty("data").GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSale_ShouldReturn200()
    {
        var saleId = await CreateTestSaleAsync();

        var response = await _client.GetAsync($"/api/sales/{saleId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListSales_ShouldReturn200()
    {
        await CreateTestSaleAsync();

        var response = await _client.GetAsync("/api/sales");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact(DisplayName = "PUT /api/sales/{id} should update sale and return 200 OK")]
    public async Task UpdateSale_ShouldReturn200()
    {
        var saleId = await CreateTestSaleAsync();
        var updateRequest = new {
            SaleNumber = "UPDATED-123",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Leo Atualizado",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Nova",
            Items = new[] {
                new { ProductId = Guid.NewGuid(), ProductName = "Produto Novo", Quantity = 5, UnitPrice = 10m }
            }
        };

        var response = await _client.PutAsJsonAsync($"/api/sales/{saleId}", updateRequest);

        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Erro: {body}");
    }

    [Fact]
    public async Task UpdateSale_NotFound_ShouldReturn404()
    {
        var request = new
        {
            saleNumber = "TEST",
            saleDate = DateTime.UtcNow,
            customerId = Guid.NewGuid(),
            customerName = "Test",
            branchId = Guid.NewGuid(),
            branchName = "Test",
            items = new[]
            {
                new
                {
                    productId = Guid.NewGuid(),
                    productName = "Produto",
                    quantity = 1,
                    unitPrice = 10m
                }
            }
        };

        var response = await _client.PutAsJsonAsync($"/api/sales/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSale_ShouldReturn200()
    {
        var saleId = await CreateTestSaleAsync();

        var response = await _client.DeleteAsync($"/api/sales/{saleId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        json.GetProperty("success").GetBoolean().Should().BeTrue();
    }
}