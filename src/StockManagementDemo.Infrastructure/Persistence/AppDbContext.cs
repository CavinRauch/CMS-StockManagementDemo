using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockManagementDemo.Core.Entities;
using StockManagementDemo.Infrastructure.Persistance.Auth;

namespace StockManagementDemo.Infrastructure.Persistance;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		RenameIdentityTables(modelBuilder);
		RegisterIEntityTypes(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}

	private void RenameIdentityTables(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ApplicationUser>(b => b.ToTable("Users"));
		modelBuilder.Entity<IdentityRole<int>>(b => b.ToTable("Roles"));
		modelBuilder.Entity<IdentityUserRole<int>>(b => b.ToTable("UserRoles"));
		modelBuilder.Entity<IdentityUserClaim<int>>(b => b.ToTable("UserClaims"));
		modelBuilder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("RoleClaims"));
		modelBuilder.Entity<IdentityUserLogin<int>>(b => b.ToTable("UserLogins"));
		modelBuilder.Entity<IdentityUserToken<int>>(b => b.ToTable("UserTokens"));
	}

	private void RegisterIEntityTypes(ModelBuilder modelBuilder)
	{
		var entityTypes = typeof(IEntity).Assembly.GetTypes().Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

		foreach (var type in entityTypes)
		{
			modelBuilder.Entity(type);
		}
	}
}
