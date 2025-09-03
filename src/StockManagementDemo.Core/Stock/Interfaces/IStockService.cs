using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Interfaces;

public interface IStockService
{
	Task<PagedResult<StockListItemDto>> GetAsync(StockQuery q, CancellationToken ct);
	Task<StockDetailDto?> GetByIdAsync(int id, CancellationToken ct);
	Task<int> CreateAsync(CreateStockRequest req, CancellationToken ct);
	Task UpdateAsync(int id, UpdateStockRequest req, CancellationToken ct);
	Task DeleteAsync(int id, CancellationToken ct);
	Task<int> AddImageAsync(int id, AddImageRequest req, CancellationToken ct);
	Task RemoveImageAsync(int id, int imageId, CancellationToken ct);
	Task SetPrimaryImageAsync(int id, int imageId, CancellationToken ct);
}