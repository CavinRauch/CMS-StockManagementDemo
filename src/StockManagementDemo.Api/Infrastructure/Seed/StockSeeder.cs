using StockManagementDemo.Core.Entities.Stock;
using StockManagementDemo.Infrastructure.Persistance;

namespace StockManagementDemo.Api.Infrastructure.Seed;

public static class StockSeeder
{
	private static readonly byte[] PlaceholderPng = Convert.FromBase64String(
		"iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII="
	);

	public static async Task SeedAsync(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		if (db.Set<StockItem>().Any())
		{
			return;
		}

		var makes = new[]
		{
			("Toyota","Corolla"),
			("Toyota","Hilux"),
			("Toyota","RAV4"),
			("Volkswagen","Polo"),
			("Volkswagen","Golf"),
			("Volkswagen","Tiguan"),
			("Suzuki","Swift"),
			("Suzuki","Vitara")
		};

		var colours = new[] { "White", "Silver", "Grey", "Black", "Blue", "Red" };
		var rand = new Random(2025);

		var items = new List<StockItem>(makes.Length);

		for (var i = 0; i < makes.Length; i++)
		{
			var (make, model) = makes[i];
			var year = rand.Next(2015, DateTime.UtcNow.Year + 1);
			var kms = rand.Next(10_000, 180_000);
			var colour = colours[rand.Next(colours.Length)];
			var regNo = $"{make[..1]}{model[..1]}-{rand.Next(10000, 99999)}";
			var vin = $"VIN{rand.Next(1000000, 9999999)}{i:D2}";

			var cost = rand.Next(80_000, 350_000);
			var retail = cost + rand.Next(20_000, 120_000);

			var item = new StockItem
			{
				// Id = (int) – let DB assign
				Make = make,
				Model = model,
				ModelYear = year,
				RegNo = regNo,
				Kms = kms,
				Colour = colour,
				Vin = vin,
				CostPrice = cost,
				RetailPrice = retail,
				Accessories = new List<StockAccessory>
				{
					new() { Name = "Aircon", Description = "Automatic climate control" },
					new() { Name = "Bluetooth", Description = "Hands-free connectivity" }
				},
				Images = new List<StockImage>
				{
					new()
					{
						Name = "placeholder.png",
						IsPrimary = true,
						ContentType = "image/png",
						Content = PlaceholderPng
					}
				}
			};

			items.Add(item);
		}

		await db.AddRangeAsync(items);
		await db.SaveChangesAsync();
	}
}
