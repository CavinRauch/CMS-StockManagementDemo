namespace StockManagementDemo.Core.Stock.Models;
public class PagedResult<T>
{
	public List<T> Items { get; set; }
	public int Total { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }

	public PagedResult(List<T> items, int total, int page, int pageSize)
	{
		Items = items;
		Total = total;
		Page = page;
		PageSize = pageSize;
	}
}
