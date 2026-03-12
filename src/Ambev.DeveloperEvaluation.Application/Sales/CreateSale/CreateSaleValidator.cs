using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.SaleNumber).NotEmpty().WithMessage("SaleNumber is required.");
        RuleFor(x => x.SaleDate).NotEmpty().WithMessage("SaleDate is required.");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("CustomerName is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("BranchId is required.");
        RuleFor(x => x.BranchName).NotEmpty().WithMessage("BranchName is required.");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemValidator());
    }
}

public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("ProductName is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("UnitPrice cannot be negative.");
    }
}