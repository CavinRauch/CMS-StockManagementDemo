namespace StockManagementDemo.Core.Stock.Models;

public class StockListItemDto
{
	public int Id { get; set; }
	public string RegNo { get; set; } = null!;
	public string Make { get; set; } = null!;
	public string Model { get; set; } = null!;
	public int ModelYear { get; set; }
	public int Kms { get; set; }
	public string Colour { get; set; } = null!;
	public decimal RetailPrice { get; set; }
	public int? PrimaryImageId { get; set; }
}
