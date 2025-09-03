using FluentValidation;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Validation;

public class StockQueryValidator : AbstractValidator<StockQuery>
{
	public StockQueryValidator()
	{
		RuleFor(x => x.Page).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
		RuleFor(x => x.Q).MaximumLength(64);
		RuleFor(x => x.MinYear).GreaterThanOrEqualTo(1950).When(x => x.MinYear.HasValue);
		RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
	}
}
