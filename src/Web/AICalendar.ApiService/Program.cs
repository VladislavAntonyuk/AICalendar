using AICalendar.ApiService.Application.AI;
using AICalendar.ApiService.Application.Events;
using AICalendar.ApiService.Application.User;
using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Authentication;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<AiCalendarDbContext>("database");

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
	})
	.AddMcp(options =>
	{
		var identityOptions = builder
			.Configuration.GetSection("AzureAd")
			.Get<MicrosoftIdentityOptions>()!;

		options.ResourceMetadata = new ProtectedResourceMetadata
		{
			Resource = GetMcpServerUrl(),
			AuthorizationServers = [GetAuthorizationServerUrl(identityOptions)],
			ScopesSupported = [$"api://{identityOptions.ClientId}/access_as_user"],
		};
	})
	.AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
							 policy.AllowAnyOrigin() //WithOrigins("https://localhost:7051")
							       .AllowAnyMethod()
							       .AllowAnyHeader());
});

builder.AddUsers();
builder.AddEvents();
builder.AddAi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddMcpServer().WithToolsFromAssembly().WithHttpTransport();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	await app.ApplyMigrations();
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => "AI Calendar API. Created by Vladislav Antonyuk.");

app.MapUserRoutes();
app.MapEventRoutes();
app.MapAiRoutes();

app.MapMcp("/mcp").RequireAuthorization();

// Run the web server
app.Run();

// Helper method to get authorization server URL
static Uri GetAuthorizationServerUrl(MicrosoftIdentityOptions o) => new($"{o.Instance?.TrimEnd('/')}/{o.TenantId}/v2.0");
Uri GetMcpServerUrl() => builder.Configuration.GetValue<Uri>("AI:McpBaseUrl");