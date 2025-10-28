namespace AICalendar.Client.Infrastructure.OperationResult;

public record OperationResultError
{
	public string Message { get; set; } = string.Empty;

	public OperationResultErrorType Type { get; set; } = OperationResultErrorType.Unknown;
}