namespace AICalendar.Client;

public record OperationResultError
{
	public string Message { get; set; } = string.Empty;

	public OperationResultErrorType Type { get; set; } = OperationResultErrorType.Unknown;
}