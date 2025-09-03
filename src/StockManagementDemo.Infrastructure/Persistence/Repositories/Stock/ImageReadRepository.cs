using Microsoft.EntityFrameworkCore;
using StockManagementDemo.Core.Entities.Stock;
using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Core.Stock.Models;
using StockManagementDemo.Infrastructure.Persistance;

namespace StockManagementDemo.Infrastructure.Persistence.Repositories.Stock;

public sealed class ImageReadRepository(AppDbContext db) : IImageReadRepository
{
	public async Task<ImageContent?> GetAsync(int stockId, int imageId, CancellationToken ct)
	{
		var row = await db.Set<StockImage>()
			.Where(i => i.Id == imageId && i.StockItemId == stockId)
			.Select(i => new { i.Content, i.ContentType })
			.FirstOrDefaultAsync(ct);

		return row is null ? null : new ImageContent { Content = row.Content, ContentType = row.ContentType };
	}

	public async Task<ImageContent?> GetPrimaryAsync(int stockId, CancellationToken ct)
	{
		var row = await db.Set<StockImage>()
			.Where(i => i.StockItemId == stockId && i.IsPrimary)
			.OrderByDescending(i => i.Id)
			.Select(i => new { i.Content, i.ContentType })
			.FirstOrDefaultAsync(ct);

		return row is null ? null : new ImageContent { Content = row.Content, ContentType = row.ContentType };
	}
}