using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Services;

public sealed class ImageService(IImageReadRepository repo) : IImageService
{
	public Task<ImageContent?> GetAsync(int stockId, int imageId, CancellationToken ct)
	{
		return repo.GetAsync(stockId, imageId, ct);
	}

	public Task<ImageContent?> GetPrimaryAsync(int stockId, CancellationToken ct)
	{
		return repo.GetPrimaryAsync(stockId, ct);
	}
}
