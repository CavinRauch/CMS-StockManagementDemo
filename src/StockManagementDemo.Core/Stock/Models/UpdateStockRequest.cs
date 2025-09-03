namespace StockManagementDemo.Core.Stock.Models;

public class UpdateStockRequest
{
	public string RegNo { get; set; } = null!;
	public string Make { get; set; } = null!;
	public string Model { get; set; } = null!;
	public int ModelYear { get; set; }
	public int Kms { get; set; }
	public string Colour { get; set; } = null!;
	public string Vin { get; set; } = null!;
	public decimal RetailPrice { get; set; }
	public decimal CostPrice { get; set; }

	public List<CreateAccessory> Accessories { get; set; } = new();
}
