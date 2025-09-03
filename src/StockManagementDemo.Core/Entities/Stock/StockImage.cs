namespace StockManagementDemo.Core.Entities.Stock;

public class StockImage : IEntity
{
	public int Id { get; set; }
	public int StockItemId { get; set; }
	public string Name { get; set; } = null!;
	public bool IsPrimary { get; set; }
	public byte[] Content { get; set; } = null!;
	public string ContentType { get; set; } = null!;

	public StockItem StockItem { get; set; } = null!;
}
