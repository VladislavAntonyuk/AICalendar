namespace AICalendar.Client.Application.Auth;

internal class AzureAdConfiguration
{
	public const string SectionName = "AzureAd";

	public required string ClientId { get; set; }
	public required string Authority { get; set; }
	public required IEnumerable<string> Scopes { get; set; }
}