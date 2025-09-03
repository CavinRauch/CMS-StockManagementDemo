using StockManagementDemo.Core.Entities.Stock;

namespace StockManagementDemo.Core.Stock.Interfaces;

public interface IStockRepository
{
	IQueryable<StockItem> Query();
	Task<StockItem?> GetAggregateAsync(int id, CancellationToken ct);
	Task AddAsync(StockItem item, CancellationToken ct);
	void Remove(StockItem item);
	Task<bool> AnyRegNoAsync(string regNo, int? exceptId, CancellationToken ct);
	Task<bool> AnyVinAsync(string vin, int? exceptId, CancellationToken ct);
	Task SaveChangesAsync(CancellationToken ct);
}
