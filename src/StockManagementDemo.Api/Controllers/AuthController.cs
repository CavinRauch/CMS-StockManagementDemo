using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockManagementDemo.Api.Auth;
using StockManagementDemo.Infrastructure.Persistance.Auth;

namespace StockManagementDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
	UserManager<ApplicationUser> userManager,
	RoleManager<IdentityRole<int>> roleManager,
	ITokenService tokenService) : ControllerBase
{
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterRequest req)
	{
		var user = new ApplicationUser { UserName = req.UserName, Email = req.Email, EmailConfirmed = true };
		var result = await userManager.CreateAsync(user, req.Password);
		if (!result.Succeeded)
		{
			return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
		}

		// Ensure default roles exist; assign Sales by default
		if (!await roleManager.RoleExistsAsync("Sales"))
		{
			await roleManager.CreateAsync(new IdentityRole<int>("Sales"));
		}

		await userManager.AddToRoleAsync(user, "Sales");

		return CreatedAtAction(nameof(Me), new { }, new { user = user.UserName, role = "Sales" });
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult<LoginResponse>> Login(LoginRequest req)
	{
		var user = await userManager.FindByNameAsync(req.UserName)
				   ?? await userManager.FindByEmailAsync(req.UserName);

		if (user is null)
		{
			return Unauthorized();
		}

		var ok = await userManager.CheckPasswordAsync(user, req.Password);
		if (!ok)
		{
			return Unauthorized();
		}

		var roles = await userManager.GetRolesAsync(user);
		var token = tokenService.Create(user, roles, out var expiresUtc);

		return Ok(new LoginResponse(token, expiresUtc, roles));
	}

	[HttpGet("me")]
	[Authorize]
	public IActionResult Me()
	{
		var name = User.Identity?.Name ?? "unknown";
		var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();
		return Ok(new { name, roles });
	}
}

public sealed record RegisterRequest(
	[Required, MinLength(3)] string UserName,
	[Required, EmailAddress] string Email,
	[Required, MinLength(8)] string Password
);

public sealed record LoginRequest(
	[Required] string UserName,
	[Required] string Password
);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, IEnumerable<string> Roles);