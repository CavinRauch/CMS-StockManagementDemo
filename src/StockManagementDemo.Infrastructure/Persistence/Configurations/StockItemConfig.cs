using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementDemo.Core.Entities.Stock;

namespace StockManagementDemo.Infrastructure.Persistance.Configurations;

public sealed class StockItemConfig : IEntityTypeConfiguration<StockItem>
{
	public void Configure(EntityTypeBuilder<StockItem> b)
	{
		b.ToTable("StockItems");

		b.HasKey(x => x.Id);

		b.Property(x => x.RegNo).IsRequired().HasMaxLength(32);
		b.Property(x => x.Make).IsRequired().HasMaxLength(64);
		b.Property(x => x.Model).IsRequired().HasMaxLength(64);
		b.Property(x => x.Colour).IsRequired().HasMaxLength(32);
		b.Property(x => x.Vin).IsRequired().HasMaxLength(32);

		b.Property(x => x.ModelYear).IsRequired();
		b.Property(x => x.Kms).IsRequired();

		b.Property(x => x.RetailPrice).HasColumnType("decimal(18,2)");
		b.Property(x => x.CostPrice).HasColumnType("decimal(18,2)");

		b.Property(x => x.DtCreated).HasDefaultValueSql("SYSUTCDATETIME()");
		b.Property(x => x.DtUpdated).HasDefaultValueSql("SYSUTCDATETIME()");

		b.HasIndex(x => x.RegNo).IsUnique();
		b.HasIndex(x => x.Vin).IsUnique();

		b.HasMany(x => x.Accessories)
			.WithOne(a => a.StockItem)
			.HasForeignKey(a => a.StockItemId)
			.OnDelete(DeleteBehavior.Cascade);

		b.HasMany(x => x.Images)
			.WithOne(i => i.StockItem)
			.HasForeignKey(i => i.StockItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
