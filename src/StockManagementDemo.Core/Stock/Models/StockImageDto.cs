namespace StockManagementDemo.Core.Stock.Models;

public class StockImageDto
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public bool IsPrimary { get; set; }
	public string ContentType { get; set; } = null!;
	public int BytesLength { get; set; }
}