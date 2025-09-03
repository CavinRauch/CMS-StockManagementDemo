using System.ComponentModel.DataAnnotations;

namespace StockManagementDemo.Api.Auth;

public sealed class JwtOptions
{
	[Required]
	public string Issuer { get; init; } = null!;

	[Required]
	public string Audience { get; init; } = null!;

	[Required, MinLength(32)]
	public string Key { get; init; } = null!;
}