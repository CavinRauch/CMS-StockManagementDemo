namespace StockManagementDemo.Core.Stock.Models;

public static class StockDtos
{
	public static StockListItemDto ToListDto(Entities.Stock.StockItem s)
	{
		return new StockListItemDto
		{
			Id = s.Id,
			RegNo = s.RegNo,
			Make = s.Make,
			Model = s.Model,
			ModelYear = s.ModelYear,
			Kms = s.Kms,
			Colour = s.Colour,
			RetailPrice = s.RetailPrice,
			PrimaryImageId = s.Images.FirstOrDefault(i => i.IsPrimary)?.Id
		};
	}

	public static StockDetailDto ToDetailDto(Entities.Stock.StockItem s)
	{
		return new StockDetailDto
		{
			Id = s.Id,
			RegNo = s.RegNo,
			Make = s.Make,
			Model = s.Model,
			ModelYear = s.ModelYear,
			Kms = s.Kms,
			Colour = s.Colour,
			Vin = s.Vin,
			RetailPrice = s.RetailPrice,
			CostPrice = s.CostPrice,
			DtCreated = s.DtCreated,
			DtUpdated = s.DtUpdated,
			Accessories = [.. s.Accessories.Select(a => new StockAccessoryDto() {
				Id = a.Id,
				Description = a.Description,
				Name = a.Name
			})],
			Images = [.. s.Images.Select(i => new StockImageDto {
				Id = i.Id,
				Name= i.Name,
				IsPrimary = i.IsPrimary,
				ContentType = i.ContentType,
				BytesLength = i.Content.Length
			})]
		};
	}
}
