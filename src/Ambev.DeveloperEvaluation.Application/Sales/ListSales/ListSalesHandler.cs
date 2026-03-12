using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var page = command.Page > 0 ? command.Page : 1;
        var size = command.Size > 0 ? command.Size : 10;

        var (sales, totalCount) = await _saleRepository.GetAllPaginatedAsync(page, size, cancellationToken);

        return new ListSalesResult
        {
            Data = _mapper.Map<List<GetSaleResult>>(sales), 
            TotalCount = totalCount,
            CurrentPage = page,
            Size = size
        };
    }
}