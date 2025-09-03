namespace StockManagementDemo.Core.Stock.Models;

public class StockDetailDto
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
	public List<StockAccessoryDto> Accessories { get; set; } = new();
	public List<StockImageDto> Images { get; set; } = new();
}
