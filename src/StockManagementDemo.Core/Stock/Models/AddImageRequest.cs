namespace StockManagementDemo.Core.Stock.Models;

public class AddImageRequest
{
	public string Name { get; set; } = null!;
	public bool IsPrimary { get; set; }
	public byte[] Content { get; set; } = null!;
	public string ContentType { get; set; } = null!;
}