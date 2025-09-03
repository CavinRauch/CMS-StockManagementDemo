using FluentValidation;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Validation;

public class CreateStockRequestValidator : AbstractValidator<CreateStockRequest>
{
	public CreateStockRequestValidator()
	{
		RuleFor(x => x.RegNo).NotEmpty().MaximumLength(32);
		RuleFor(x => x.Make).NotEmpty().MaximumLength(64);
		RuleFor(x => x.Model).NotEmpty().MaximumLength(64);
		RuleFor(x => x.Colour).NotEmpty().MaximumLength(32);
		RuleFor(x => x.Vin).NotEmpty().MaximumLength(32);

		RuleFor(x => x.ModelYear)
			.InclusiveBetween(1950, DateTime.UtcNow.Year + 1);
		RuleFor(x => x.Kms)
			.GreaterThanOrEqualTo(0);

		RuleFor(x => x.CostPrice)
			.GreaterThanOrEqualTo(0);
		RuleFor(x => x.RetailPrice)
			.GreaterThanOrEqualTo(0)
			.GreaterThanOrEqualTo(x => x.CostPrice)
			.WithMessage("RetailPrice must be >= CostPrice");

		RuleForEach(x => x.Accessories)
			.SetValidator(new CreateAccessoryValidator());
	}
}
