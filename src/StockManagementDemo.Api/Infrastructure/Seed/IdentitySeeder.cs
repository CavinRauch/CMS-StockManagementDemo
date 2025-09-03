using Microsoft.AspNetCore.Identity;
using StockManagementDemo.Infrastructure.Persistance.Auth;

namespace StockManagementDemo.Api.Infrastructure.Seed;

public static class IdentitySeeder
{
	public static async Task SeedAsync(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
		var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		string[] roles = ["Admin", "Sales", "Viewer"];
		foreach (var r in roles)
		{
			if (!await roleMgr.RoleExistsAsync(r))
			{
				await roleMgr.CreateAsync(new IdentityRole<int>(r));
			}
		}

		var adminEmail = "admin@stockdemo.test";
		var admin = await userMgr.FindByEmailAsync(adminEmail);
		if (admin is null)
		{
			admin = new ApplicationUser { UserName = "admin", Email = adminEmail, EmailConfirmed = true };
			await userMgr.CreateAsync(admin, "P@ssw0rd!");
			await userMgr.AddToRolesAsync(admin, roles);
		}
	}
}