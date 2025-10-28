namespace AICalendar.ApiService.Infrastructure.Extensions;

public class AiSettings
{
	public required Uri Endpoint { get; init; }
	public required string Key { get; init; }
	public required string Model { get; init; }
	public required Uri McpBaseUrl { get; init; }
}