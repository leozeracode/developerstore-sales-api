using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Handle should return paginated list of sales")]
    public async Task Handle_ValidRequest_ShouldReturnPaginatedResult()
    {
        var command = new ListSalesCommand { Page = 2, Size = 5 };
        var salesList = new List<Sale> { new Sale("SALE-1", Guid.NewGuid(), "C1", Guid.NewGuid(), "B1") };
        var totalCount = 12; 

        _saleRepository.GetAllPaginatedAsync(command.Page, command.Size, Arg.Any<CancellationToken>())
            .Returns((salesList, totalCount));

        _mapper.Map<List<GetSaleResult>>(salesList).Returns(new List<GetSaleResult> { new GetSaleResult() });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(2);
        result.Size.Should().Be(5);
        result.TotalCount.Should().Be(12);
        result.TotalPages.Should().Be(3); 
    }
}