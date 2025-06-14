using AICalendar.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowApiClient", policy =>
	{
		policy.AllowAnyOrigin()//.WithOrigins("https://localhost:5002") // ApiService URL
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

builder.Services
	   .AddMcpServer()
	   .WithHttpTransport()
	   .WithToolsFromAssembly();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseCors("AllowApiClient");
app.MapMcp();

await app.RunAsync();
