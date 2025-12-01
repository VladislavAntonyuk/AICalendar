using System.Runtime.CompilerServices;
using AICalendar.ApiService.Infrastructure.Extensions;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace AICalendar.ApiService.Application.AI;

internal sealed class AiHandler(IChatClient client, IOptions<AiSettings> settings, IHttpContextAccessor context)
{
	public async IAsyncEnumerable<AgentRunResponseUpdate> Handle(
		Guid currentUserId,
		string prompt,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		if (context.HttpContext is null)
		{
			yield break;
		}

		var accessToken = await context.HttpContext.GetTokenAsync("access_token");
		var mcpClient = await McpClient.CreateAsync(new HttpClientTransport(
													  new HttpClientTransportOptions
													  {
														  Endpoint = settings.Value.McpBaseUrl,
														  Name = "AICalendar.ApiService",
														  AdditionalHeaders = new Dictionary<string, string>
														  {
															  ["Authorization"] = $"Bearer {accessToken}"
														  }
													  }), cancellationToken: cancellationToken);
		var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

		List<ChatMessage> messages = [new(ChatRole.User, prompt)];
		await foreach (var chunk in client.CreateAIAgent(
			                                  "You are a helpful assistant for managing calendar events",
			                                  "AI Calendar Agent",
			                                  "AI Agent for managing calendar events",
											  tools.Cast<AITool>().ToList())
		                                  .RunStreamingAsync(messages, cancellationToken: cancellationToken))
		{
			yield return chunk;
		}
	}
}