using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListSalesResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSales([FromQuery] int _page = 1, [FromQuery] int _size = 10, CancellationToken cancellationToken = default)
    {
        var command = new ListSalesCommand { Page = _page, Size = _size };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<ListSalesResult>
        {
            Success = true,
            Message = "Sales retrieved successfully",
            Data = response
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetSaleCommand(id);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);
            var result = _mapper.Map<CreateSaleResponse>(response);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            request.Id = id;
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateSaleResult>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSaleCommand(id);
        var success = await _mediator.Send(command, cancellationToken);

        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Sale cancelled successfully"
        });
    }
    
    [HttpDelete("{saleId}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSaleItem([FromRoute] Guid saleId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var command = new CancelItemCommand(saleId, itemId);
        var success = await _mediator.Send(command, cancellationToken);

        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Sale or Item not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Item cancelled successfully"
        });
    }
}