namespace StockManagementDemo.Core.Entities.Stock;

public class StockItem : IEntity
{
	public int Id { get; set; }
	public string RegNo { get; set; } = null!;
	public string Make { get; set; } = null!;
	public string Model { get; set; } = null!;
	public int ModelYear { get; set; }
	public int Kms { get; set; }
	public string Colour { get; set; } = null!;
	public string Vin { get; set; } = null!;
	public decimal RetailPrice { get; set; }
	public decimal CostPrice { get; set; }
	public DateTime DtCreated { get; set; }
	public DateTime DtUpdated { get; set; }
	public ICollection<StockAccessory> Accessories { get; set; } = new List<StockAccessory>();
	public ICollection<StockImage> Images { get; set; } = new List<StockImage>();
}
