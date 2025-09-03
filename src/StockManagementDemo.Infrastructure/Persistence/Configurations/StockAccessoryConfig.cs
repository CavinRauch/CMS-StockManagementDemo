using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementDemo.Core.Entities.Stock;

namespace StockManagementDemo.Infrastructure.Persistance.Configurations;

public sealed class StockAccessoryConfig : IEntityTypeConfiguration<StockAccessory>
{
	public void Configure(EntityTypeBuilder<StockAccessory> b)
	{
		b.ToTable("StockAccessories");

		b.HasKey(x => x.Id);

		b.Property(x => x.Name).IsRequired().HasMaxLength(64);
		b.Property(x => x.Description).HasMaxLength(256);
	}
}
