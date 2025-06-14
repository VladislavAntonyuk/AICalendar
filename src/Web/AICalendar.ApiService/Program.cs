using AICalendar.ApiService.Application.AI;
using AICalendar.ApiService.Application.Events;
using AICalendar.ApiService.Application.User;
using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<AiCalendarDbContext>("database");

builder.Services.AddProblemDetails();
builder.AddUsers();
builder.AddEvents();
await builder.AddAi(builder.Configuration.GetSection("AI").Get<AiSettings>());
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		                         policy.AllowAnyOrigin() //WithOrigins("https://localhost:7051")
		                               .AllowAnyMethod()
		                               .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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

app.MapUserRoutes();
app.MapEventRoutes();
app.MapAiRoutes();

await app.RunAsync();