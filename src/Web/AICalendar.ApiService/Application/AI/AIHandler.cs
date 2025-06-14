using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace AICalendar.ApiService.Application.AI;

internal sealed class AiHandler(IChatClient client, ChatOptions options)
{
	public IAsyncEnumerable<ChatResponseUpdate> Handle(
		Guid currentUserId,
		string prompt,
		CancellationToken cancellationToken = default)
	{
		List<ChatMessage> messages = [new(ChatRole.User, prompt)];

		return client.GetStreamingResponseAsync(messages, options, cancellationToken);
	}
}