namespace StockManagementDemo.Core.Stock.Models;

public class StockQuery
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public string? Q { get; set; }
	public int? MinYear { get; set; }
	public decimal? MaxPrice { get; set; }
	public string? Sort { get; set; }
}