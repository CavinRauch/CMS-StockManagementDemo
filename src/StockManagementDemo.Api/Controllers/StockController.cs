using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Core.Stock.Models;

namespace StockManagementDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController(
	IStockService service,
	IImageService imageService) : ControllerBase
{
	// GET /api/stock?page=1&pageSize=20&sort=price&q=hilux&minYear=2018&maxPrice=400000
	[HttpGet]
	public async Task<ActionResult<PagedResult<StockListItemDto>>> Get([FromQuery] StockQuery query, CancellationToken ct)
	{
		return Ok(await service.GetAsync(query, ct));
	}

	// GET /api/stock/123
	[HttpGet("{id:int}")]
	public async Task<ActionResult<StockDetailDto>> GetById(int id, CancellationToken ct)
	{
		var dto = await service.GetByIdAsync(id, ct);
		return dto is null ? NotFound() : Ok(dto);
	}

	// POST /api/stock
	[HttpPost]
	[Authorize(Roles = "Admin,Sales")]
	public async Task<ActionResult> Create([FromBody] CreateStockRequest req, CancellationToken ct)
	{
		var id = await service.CreateAsync(req, ct);
		return CreatedAtAction(nameof(GetById), new { id }, new { id });
	}

	// PUT /api/stock/123
	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin,Sales")]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateStockRequest req, CancellationToken ct)
	{
		await service.UpdateAsync(id, req, ct);
		return NoContent();
	}

	// DELETE /api/stock/123
	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id, CancellationToken ct)
	{
		await service.DeleteAsync(id, ct);
		return NoContent();
	}

	// POST /api/stock/123/images (multipart/form-data)
	[HttpPost("{id:int}/images")]
	[Authorize(Roles = "Admin,Sales")]
	[Consumes("multipart/form-data")]
	[RequestSizeLimit(10_000_000)]
	public async Task<ActionResult> AddImage(int id, [FromForm] UploadImageRequest req, CancellationToken ct)
	{
		if (req.File is null || req.File.Length == 0)
		{
			return BadRequest(new { error = "No file uploaded." });
		}

		await using var ms = new MemoryStream();
		await req.File.CopyToAsync(ms, ct);

		var imageId = await service.AddImageAsync(id, new AddImageRequest
		{
			Name = req.Name,
			IsPrimary = req.IsPrimary,
			Content = ms.ToArray(),
			ContentType = req.File.ContentType
		}, ct);

		return CreatedAtAction(nameof(GetImage), new { id, imageId }, new { imageId });
	}

	// GET /api/stock/123/images/456
	[HttpGet("{id:int}/images/{imageId:int}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetImage(int id, int imageId, CancellationToken ct)
	{
		var img = await imageService.GetAsync(id, imageId, ct);
		return img is null ? NotFound() : File(img.Content, img.ContentType);
	}

	// Convenience: GET /api/stock/123/primary-image
	[HttpGet("{id:int}/primary-image")]
	[AllowAnonymous]
	public async Task<IActionResult> GetPrimaryImage(int id, CancellationToken ct)
	{
		var img = await imageService.GetPrimaryAsync(id, ct);
		return img is null ? NotFound() : File(img.Content, img.ContentType);
	}

	// PUT /api/stock/{id}/images/{imageId}/primary
	[HttpPut("{id:int}/images/{imageId:int}/primary")]
	[Authorize(Roles = "Admin,Sales")]
	public async Task<IActionResult> SetPrimaryImage(int id, int imageId, CancellationToken ct)
	{
		await service.SetPrimaryImageAsync(id, imageId, ct);
		return NoContent();
	}

	// DELETE /api/stock/123/images/456
	[HttpDelete("{id:int}/images/{imageId:int}")]
	[Authorize(Roles = "Admin,Sales")]
	public async Task<IActionResult> RemoveImage(int id, int imageId, CancellationToken ct)
	{
		await service.RemoveImageAsync(id, imageId, ct);
		return NoContent();
	}
}

public record UploadImageRequest(IFormFile File, string Name, bool IsPrimary);
