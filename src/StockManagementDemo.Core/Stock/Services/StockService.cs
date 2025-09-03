using StockManagementDemo.Core.Common;
using StockManagementDemo.Core.Entities.Stock;
using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Core.Stock.Services;
public sealed class StockService(IStockRepository repo) : IStockService
{
	public async Task<PagedResult<StockListItemDto>> GetAsync(StockQuery q, CancellationToken ct)
	{
		var query = repo.Query();

		if (!string.IsNullOrWhiteSpace(q.Q))
		{
			var like = q.Q.Trim();
			query = query.Where(s =>
				s.Make.Contains(like) || s.Model.Contains(like) ||
				s.RegNo.Contains(like) || s.Vin.Contains(like));
		}
		if (q.MinYear is not null)
		{
			query = query.Where(s => s.ModelYear >= q.MinYear);
		}

		if (q.MaxPrice is not null)
		{
			query = query.Where(s => s.RetailPrice <= q.MaxPrice);
		}

		query = q.Sort switch
		{
			"make" => query.OrderBy(s => s.Make),
			"-make" => query.OrderByDescending(s => s.Make),
			"model" => query.OrderBy(s => s.Model),
			"-model" => query.OrderByDescending(s => s.Model),
			"modelYear" => query.OrderBy(s => s.ModelYear),
			"-modelYear" => query.OrderByDescending(s => s.ModelYear),
			"price" => query.OrderBy(s => s.RetailPrice),
			"-price" => query.OrderByDescending(s => s.RetailPrice),
			_ => query.OrderByDescending(s => s.DtCreated)
		};

		var items = query
			.Skip((q.Page - 1) * q.PageSize)
			.Take(q.PageSize)
			.Select(StockDtos.ToListDto)
			.ToList();
		var total = items.Count;

		return new(items, total, q.Page, q.PageSize);
	}

	public async Task<StockDetailDto?> GetByIdAsync(int id, CancellationToken ct)
	{
		var s = await repo.GetAggregateAsync(id, ct);

		return s is null ? null : StockDtos.ToDetailDto(s);
	}

	public async Task<int> CreateAsync(CreateStockRequest req, CancellationToken ct)
	{
		await EnsureValidAndUniqueAsync(req.RegNo, req.Vin, null, req.RetailPrice, req.CostPrice, ct);

		var entity = new StockItem
		{
			RegNo = req.RegNo.Trim(),
			Make = req.Make.Trim(),
			Model = req.Model.Trim(),
			ModelYear = req.ModelYear,
			Kms = req.Kms,
			Colour = req.Colour.Trim(),
			Vin = req.Vin.Trim(),
			RetailPrice = req.RetailPrice,
			CostPrice = req.CostPrice,
			DtCreated = DateTime.UtcNow,
			DtUpdated = DateTime.UtcNow,
			Accessories = req.Accessories.Select(a => new StockAccessory
			{
				Name = a.Name.Trim(),
				Description = a.Description
			}).ToList()
		};

		await repo.AddAsync(entity, ct);
		await repo.SaveChangesAsync(ct);

		return entity.Id;
	}

	public async Task UpdateAsync(int id, UpdateStockRequest req, CancellationToken ct)
	{
		var entity = await repo.GetAggregateAsync(id, ct) ?? throw new NotFoundException("stock");
		await EnsureValidAndUniqueAsync(req.RegNo, req.Vin, id, req.RetailPrice, req.CostPrice, ct);

		entity.RegNo = req.RegNo.Trim();
		entity.Make = req.Make.Trim();
		entity.Model = req.Model.Trim();
		entity.ModelYear = req.ModelYear;
		entity.Kms = req.Kms;
		entity.Colour = req.Colour.Trim();
		entity.Vin = req.Vin.Trim();
		entity.RetailPrice = req.RetailPrice;
		entity.CostPrice = req.CostPrice;
		entity.DtUpdated = DateTime.UtcNow;

		entity.Accessories.Clear();
		foreach (var a in req.Accessories)
		{
			entity.Accessories.Add(new StockAccessory { StockItemId = id, Name = a.Name.Trim(), Description = a.Description });
		}

		await repo.SaveChangesAsync(ct);
	}

	public async Task DeleteAsync(int id, CancellationToken ct)
	{
		var entity = await repo.GetAggregateAsync(id, ct) ?? throw new NotFoundException("stock");
		repo.Remove(entity);

		await repo.SaveChangesAsync(ct);
	}

	public async Task<int> AddImageAsync(int id, AddImageRequest req, CancellationToken ct)
	{
		if (req.Content is null || req.Content.Length == 0)
		{
			throw new DomainRuleException("Image content is empty.");
		}

		if (!req.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
		{
			throw new DomainRuleException("Only image content types are allowed.");
		}

		var entity = await repo.GetAggregateAsync(id, ct) ?? throw new NotFoundException("stock");
		if (entity.Images.Count >= 3)
		{
			throw new DomainRuleException("Max 3 images per vehicle.");
		}

		var img = new StockImage
		{
			StockItemId = id,
			Name = req.Name.Trim(),
			IsPrimary = req.IsPrimary,
			Content = req.Content,
			ContentType = req.ContentType
		};

		if (img.IsPrimary)
		{
			foreach (var i in entity.Images)
			{
				i.IsPrimary = false;
			}
		}

		entity.Images.Add(img);
		await repo.SaveChangesAsync(ct);
		return img.Id;
	}

	public async Task RemoveImageAsync(int id, int imageId, CancellationToken ct)
	{
		var entity = await repo.GetAggregateAsync(id, ct) ?? throw new NotFoundException("stock");
		var img = entity.Images.FirstOrDefault(i => i.Id == imageId) ?? throw new NotFoundException("image");

		if (img.IsPrimary)
		{
			var next = entity.Images.OrderByDescending(i => i.Id).FirstOrDefault();
			if (next is not null)
			{
				next.IsPrimary = true;
				await repo.SaveChangesAsync(ct);
			}
		}

		entity.Images.Remove(img);

		await repo.SaveChangesAsync(ct);
	}

	public async Task SetPrimaryImageAsync(int stockId, int imageId, CancellationToken ct)
	{
		var entity = await repo.GetAggregateAsync(stockId, ct) ?? throw new NotFoundException("stock");
		var img = entity.Images.FirstOrDefault(i => i.Id == imageId) ?? throw new NotFoundException("image");

		foreach (var i in entity.Images)
		{
			i.IsPrimary = false;
		}

		img.IsPrimary = true;

		await repo.SaveChangesAsync(ct);
	}

	private async Task EnsureValidAndUniqueAsync(string regNo, string vin, int? exceptId, decimal retail, decimal cost, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(regNo))
		{
			throw new DomainRuleException("RegNo is required.");
		}

		if (string.IsNullOrWhiteSpace(vin))
		{
			throw new DomainRuleException("VIN is required.");
		}

		if (retail < 0 || cost < 0)
		{
			throw new DomainRuleException("Prices must be non-negative.");
		}

		if (retail < cost)
		{
			throw new DomainRuleException("RetailPrice must be >= CostPrice.");
		}

		if (await repo.AnyRegNoAsync(regNo.Trim(), exceptId, ct))
		{
			throw new ConflictException("RegNo already exists.");
		}

		if (await repo.AnyVinAsync(vin.Trim(), exceptId, ct))
		{
			throw new ConflictException("VIN already exists.");
		}
	}
}
