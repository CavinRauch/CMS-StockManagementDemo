using FluentValidation;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Validation;

public class CreateAccessoryValidator : AbstractValidator<CreateAccessory>
{
	public CreateAccessoryValidator()
	{
		RuleFor(x => x.Name).NotEmpty().MaximumLength(64);
		RuleFor(x => x.Description).MaximumLength(256);
	}
}