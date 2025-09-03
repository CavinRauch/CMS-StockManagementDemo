using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StockManagementDemo.Infrastructure.Persistance.Auth;

namespace StockManagementDemo.Api.Auth;

public sealed class TokenService(JwtOptions options) : ITokenService
{
	public string Create(ApplicationUser user, IEnumerable<string> roles, out DateTime expiresUtc)
	{
		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.UserName ?? user.Email ?? "user"),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};
		claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: options.Issuer,
			audience: options.Audience,
			claims: claims,
			notBefore: DateTime.UtcNow,
			expires: DateTime.UtcNow.AddHours(2),
			signingCredentials: creds);

		expiresUtc = token.ValidTo;
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}