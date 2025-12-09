using AICalendar.ApiService.Application.AI;
using Microsoft.Extensions.AI;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class AiExtensions
{
	public static WebApplicationBuilder AddAi(this WebApplicationBuilder builder)
	{
		builder.AddAzureChatCompletionsClient("chat")
		       .AddChatClient()
		       .UseLogging()
		       .UseOpenTelemetry()
		       .UseFunctionInvocation();

		builder.Services.AddScoped<AiHandler>();
		builder.Services.AddOptions<AiSettings>().Bind(builder.Configuration.GetSection("AI")).ValidateDataAnnotations().ValidateOnStart();

		return builder;
	}
}