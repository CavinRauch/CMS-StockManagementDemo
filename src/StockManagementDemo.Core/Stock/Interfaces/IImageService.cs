using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Interfaces;
public interface IImageService
{
	Task<ImageContent?> GetAsync(int stockId, int imageId, CancellationToken ct);
	Task<ImageContent?> GetPrimaryAsync(int stockId, CancellationToken ct);
}
