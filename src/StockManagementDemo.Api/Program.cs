using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StockManagementDemo.Api.Auth;
using StockManagementDemo.Api.Infrastructure.Seed;
using StockManagementDemo.Api.Middleware;
using StockManagementDemo.Core.Stock.Interfaces;
using StockManagementDemo.Core.Stock.Services;
using StockManagementDemo.Core.Stock.Validation;
using StockManagementDemo.Infrastructure.Persistance;
using StockManagementDemo.Infrastructure.Persistance.Auth;
using StockManagementDemo.Infrastructure.Persistence.Repositories.Stock;

var builder = WebApplication.CreateBuilder(args);

// EF Core - AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
	.AddIdentityCore<ApplicationUser>(opt =>
	{
		opt.User.RequireUniqueEmail = true;
		opt.Password.RequireDigit = true;
		opt.Password.RequiredLength = 8;
		opt.Password.RequireUppercase = false;
		opt.Password.RequireLowercase = true;
		opt.Password.RequireNonAlphanumeric = false;
	})
	.AddRoles<IdentityRole<int>>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

// Bind & validate JWT options
builder.Services.AddOptions<JwtOptions>()
	.Bind(builder.Configuration.GetSection("Jwt"))
	.ValidateDataAnnotations()
	.Validate(o => o.Key.Length >= 32, "JWT key must be at least 32 chars.")
	.ValidateOnStart();

// JWT Bearer
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwt.Issuer,
			ValidAudience = jwt.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
		};
	});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITokenService, TokenService>(sp =>
{
	var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtOptions>>().Value;
	return new TokenService(options);
});
builder.Services.AddScoped<IImageReadRepository, ImageReadRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddFluentValidationAutoValidation(options =>
{
	options.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateAccessoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateStockRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<StockQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateStockRequestValidator>();

builder.Services.AddProblemDetails();

builder.Services.AddCors(o => o.AddPolicy("ng-dev", p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock Management Demo API", Version = "v1" });

	// JWT bearer scheme
	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter: Bearer {your JWT token}",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = "Bearer"
		}
	};

	c.AddSecurityDefinition("Bearer", securityScheme);

	// Require JWT auth by default (applies to all endpoints unless you override)
	var securityRequirement = new OpenApiSecurityRequirement
	{
		{
			securityScheme,
			Array.Empty<string>()
		}
	};
	c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

app.UseProblemDetailsExceptionMapping(includeStackInDevelopment: true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	// Seed identity data
	await IdentitySeeder.SeedAsync(app.Services);
	await StockSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseCors("ng-dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();