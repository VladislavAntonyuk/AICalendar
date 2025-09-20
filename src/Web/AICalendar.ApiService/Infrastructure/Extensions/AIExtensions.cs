using System.ClientModel;
using AICalendar.ApiService.Application.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class AiExtensions
{
	public static WebApplicationBuilder AddAi(this WebApplicationBuilder builder)
	{
		builder.Services.AddChatClient(sp =>
		{
			var settings = sp.GetRequiredService<IOptions<AiSettings>>().Value;
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
		builder.Services.AddOptions<AiSettings>().Bind(builder.Configuration.GetSection("AI")).ValidateDataAnnotations().ValidateOnStart();

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