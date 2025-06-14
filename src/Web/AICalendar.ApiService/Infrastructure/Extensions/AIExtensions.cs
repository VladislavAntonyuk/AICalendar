using System.ClientModel;
using AICalendar.ApiService.Application.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OpenAI;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class AiExtensions
{
	public static async Task<WebApplicationBuilder> AddAi(this WebApplicationBuilder builder, AiSettings settings)
	{
		builder.Services.AddChatClient(sp =>
		{
			var chatClientBuilder = new ChatClientBuilder(new OpenAIClient(new ApiKeyCredential(settings.Key), new OpenAIClientOptions()
			{
				Endpoint = settings.Endpoint
			}).GetChatClient(settings.Model).AsIChatClient())
									.UseLogging()
									.UseOpenTelemetry()
									.UseFunctionInvocation();

			return chatClientBuilder.Build(sp);
		});

		builder.Services.AddScoped<AiHandler>();
		var mcpClient = await McpClientFactory.CreateAsync(new SseClientTransport(
			                                                   new SseClientTransportOptions()
			                                                   {
				                                                   Endpoint = settings.McpBaseUrl,
				                                                   Name = "AICalendar.ApiService"
			                                                   }));
		var tools = await mcpClient.ListToolsAsync();
		builder.Services.AddSingleton<ChatOptions>(_ => new() { Tools = [.. tools] });
		return builder;
	}
}

public class AiSettings
{
	public required Uri Endpoint { get; init; }
	public required string Key { get; init; }
	public required string Model { get; init; }
	public required Uri McpBaseUrl { get; init; }
}