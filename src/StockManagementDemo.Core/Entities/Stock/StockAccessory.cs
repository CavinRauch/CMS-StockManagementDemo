namespace StockManagementDemo.Core.Entities.Stock;

public class StockAccessory : IEntity
{
	public int Id { get; set; }
	public int StockItemId { get; set; }
	public string Name { get; set; } = null!;
	public string? Description { get; set; }
	public StockItem StockItem { get; set; } = null!;

}
