using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagementDemo.Core.Common;

namespace StockManagementDemo.Api.Middleware;

public static class ProblemDetailsExtensions
{
	public static void UseProblemDetailsExceptionMapping(this IApplicationBuilder app, bool includeStackInDevelopment = true)
	{
		app.UseExceptionHandler(builder =>
		{
			builder.Run(async context =>
			{
				var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
				var errorFeature = context.Features.Get<IExceptionHandlerPathFeature>();
				var ex = errorFeature?.Error;

				var (status, title, type) = MapException(ex);

				var problem = new ProblemDetails
				{
					Status = status,
					Title = title,
					Type = type,
					Detail = ShouldIncludeDetail(env, includeStackInDevelopment) ? ex?.ToString() : null,
					Instance = context.Request.Path
				};

				var traceId = context.TraceIdentifier;
				problem.Extensions["traceId"] = traceId;

				context.Response.StatusCode = status;
				context.Response.ContentType = "application/problem+json";
				await context.Response.WriteAsJsonAsync(problem);
			});
		});
	}

	private static bool ShouldIncludeDetail(IHostEnvironment env, bool includeStackInDevelopment)
	{
		return includeStackInDevelopment && env.IsDevelopment();
	}

	private static (int status, string title, string type) MapException(Exception? ex)
	{
		return ex switch
		{
			NotFoundException => ((int)HttpStatusCode.NotFound, ex!.Message, "https://httpstatuses.io/404"),
			ConflictException => ((int)HttpStatusCode.Conflict, ex!.Message, "https://httpstatuses.io/409"),
			DomainRuleException => ((int)HttpStatusCode.BadRequest, ex!.Message, "https://httpstatuses.io/400"),

			UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, "Forbidden", "https://httpstatuses.io/403"),
			DbUpdateConcurrencyException => ((int)HttpStatusCode.Conflict, "Concurrency conflict", "https://httpstatuses.io/409"),
			DbUpdateException => ((int)HttpStatusCode.Conflict, "Database update failed", "https://httpstatuses.io/409"),

			_ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred", "https://httpstatuses.io/500"),
		};
	}
}
