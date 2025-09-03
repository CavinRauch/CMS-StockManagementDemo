using StockManagementDemo.Infrastructure.Persistance.Auth;

namespace StockManagementDemo.Api.Auth;

public interface ITokenService
{
	string Create(ApplicationUser user, IEnumerable<string> roles, out DateTime expiresUtc);
}
