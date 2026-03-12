using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public class CreateSaleFeatureTests : IClassFixture<WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>>
{
    private readonly HttpClient _client;

    public CreateSaleFeatureTests(WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "POST /api/sales should process entire flow and return 201 Created")]
    public async Task CreateSale_ValidPayload_ShouldReturnCreatedAndPersist()
    {
        var request = new CreateSaleRequest
        {
            SaleNumber = "FUNC-TEST-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Leonardo Funcional",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Teste",
            Items = new List<CreateSaleItemRequest>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Cerveja Funcional",
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/sales", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.SaleNumber.Should().Be("FUNC-TEST-001");

        result.Data.Id.Should().NotBeEmpty();
    }
}