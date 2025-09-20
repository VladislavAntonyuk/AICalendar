using AICalendar.ApiService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace AICalendar.ApiService.Application.AI;

internal sealed class AiHandler(IChatClient client, IOptions<AiSettings> settings, IHttpContextAccessor context)
{
	public async IAsyncEnumerable<ChatResponseUpdate> Handle(
		Guid currentUserId,
		string prompt,
		CancellationToken cancellationToken = default)
	{
		if (context.HttpContext is null)
		{
			yield break;
		}

		var accessToken = await context.HttpContext.GetTokenAsync("access_token");
		var mcpClient = await McpClientFactory.CreateAsync(new SseClientTransport(
													  new SseClientTransportOptions()
													  {
														  Endpoint = settings.Value.McpBaseUrl,
														  Name = "AICalendar.ApiService",
														  AdditionalHeaders = new Dictionary<string, string>
														  {
															  ["Authorization"] = $"Bearer {accessToken}"
														  }
													  }), cancellationToken: cancellationToken);
		IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

		List<ChatMessage> messages = [new(ChatRole.User, prompt)];
		var options = new ChatOptions() { Tools = [.. tools] };
		await foreach (var chunk in client.GetStreamingResponseAsync(messages, options, cancellationToken))
		{
			yield return chunk;
		}
	}
}