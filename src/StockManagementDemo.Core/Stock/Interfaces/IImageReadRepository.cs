using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Interfaces;

public interface IImageReadRepository
{
	/// <summary>Returns image bytes for a specific stock + image id, or null if not found/belongs to another stock.</summary>
	Task<ImageContent?> GetAsync(int stockId, int imageId, CancellationToken ct);

	/// <summary>Returns bytes for the primary image of a stock item, or null if none.</summary>
	Task<ImageContent?> GetPrimaryAsync(int stockId, CancellationToken ct);
}