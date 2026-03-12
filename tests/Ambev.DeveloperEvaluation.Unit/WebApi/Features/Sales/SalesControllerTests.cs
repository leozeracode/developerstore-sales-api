using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales;

public class SalesControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = Substitute.For<IMapper>();
        
        _controller = new SalesController(_mediator, _mapper);
    }

    [Fact(DisplayName = "CreateSale should return 201 Created when successful")]
    public async Task CreateSale_ValidRequest_ShouldReturnCreated()
    {
        var request = new CreateSaleRequest { SaleNumber = "SALE-123" };
        var command = new CreateSaleCommand { SaleNumber = "SALE-123" };
        var result = new CreateSaleResult { Id = Guid.NewGuid(), SaleNumber = "SALE-123" };

        _mapper.Map<CreateSaleCommand>(request).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(result);
        _mapper.Map<CreateSaleResponse>(result).Returns(new CreateSaleResponse { Id = result.Id });

        var response = await _controller.CreateSale(request, CancellationToken.None);

        var createdResult = response.Should().BeOfType<CreatedResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        
        var apiResponse = createdResult.Value.Should().BeOfType<ApiResponseWithData<CreateSaleResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "GetSale should return 200 OK when sale exists")]
    public async Task GetSale_ExistingSale_ShouldReturnOk()
    {
        var saleId = Guid.NewGuid();
        var result = new GetSaleResult { Id = saleId, SaleNumber = "SALE-123" };

        _mediator.Send(Arg.Any<GetSaleCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.GetSale(saleId, CancellationToken.None);

        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<GetSaleResult>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.SaleNumber.Should().Be("SALE-123");
    }

    [Fact(DisplayName = "DeleteSale should return 200 OK when deletion is successful")]
    public async Task DeleteSale_ExistingSale_ShouldReturnOk()
    {
        var saleId = Guid.NewGuid();
        
        _mediator.Send(Arg.Any<DeleteSaleCommand>(), Arg.Any<CancellationToken>()).Returns(true);

        var response = await _controller.DeleteSale(saleId, CancellationToken.None);

        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact(DisplayName = "DeleteSale should return 404 NotFound when sale does not exist")]
    public async Task DeleteSale_NonExistingSale_ShouldReturnNotFound()
    {
        var saleId = Guid.NewGuid();
        
        _mediator.Send(Arg.Any<DeleteSaleCommand>(), Arg.Any<CancellationToken>()).Returns(false);

        var response = await _controller.DeleteSale(saleId, CancellationToken.None);

        var notFoundResult = response.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }
}