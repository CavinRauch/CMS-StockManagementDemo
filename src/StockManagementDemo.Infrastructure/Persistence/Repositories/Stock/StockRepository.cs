using Microsoft.EntityFrameworkCore;
using StockManagementDemo.Core.Entities.Stock;
using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Infrastructure.Persistance;

namespace StockManagementDemo.Infrastructure.Persistence.Repositories.Stock;

public sealed class StockRepository(AppDbContext db) : IStockRepository
{
	public IQueryable<StockItem> Query()
	{
		return db.Set<StockItem>()
		  .AsNoTracking()
		  .Include(s => s.Images)
		  .Include(s => s.Accessories);
	}

	public Task<StockItem?> GetAggregateAsync(int id, CancellationToken ct)
	{
		return db.Set<StockItem>()
		  .Include(s => s.Images)
		  .Include(s => s.Accessories)
		  .FirstOrDefaultAsync(s => s.Id == id, ct);
	}

	public Task AddAsync(StockItem item, CancellationToken ct)
	{
		return db.AddAsync(item, ct).AsTask();
	}

	public void Remove(StockItem item)
	{
		db.Remove(item);
	}

	public Task SaveChangesAsync(CancellationToken ct)
	{
		return db.SaveChangesAsync(ct);
	}

	public Task<bool> AnyRegNoAsync(string regNo, int? exceptId, CancellationToken ct)
	{
		return db.Set<StockItem>().AnyAsync(s => s.RegNo == regNo && (exceptId == null || s.Id != exceptId), ct);
	}

	public Task<bool> AnyVinAsync(string vin, int? exceptId, CancellationToken ct)
	{
		return db.Set<StockItem>().AnyAsync(s => s.Vin == vin && (exceptId == null || s.Id != exceptId), ct);
	}
}
