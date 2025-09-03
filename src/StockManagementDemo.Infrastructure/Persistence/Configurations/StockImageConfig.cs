using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementDemo.Core.Entities.Stock;

namespace StockManagementDemo.Infrastructure.Persistance.Configurations;

public sealed class StockImageConfig : IEntityTypeConfiguration<StockImage>
{
	public void Configure(EntityTypeBuilder<StockImage> b)
	{
		b.ToTable("StockImages");

		b.HasKey(x => x.Id);

		b.Property(x => x.Name).IsRequired().HasMaxLength(64);
		b.Property(x => x.ContentType).IsRequired().HasMaxLength(64);

		b.Property(x => x.Content).HasColumnType("varbinary(max)");

		b.HasIndex(x => new { x.StockItemId, x.IsPrimary });
	}
}
