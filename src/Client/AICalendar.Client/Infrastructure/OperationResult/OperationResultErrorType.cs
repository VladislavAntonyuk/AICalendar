namespace AICalendar.Client.Infrastructure.OperationResult;

public enum OperationResultErrorType
{
	Unknown = 0,
	AuthenticationCanceled,
	UiInteractiveSignInRequired,
	ConnectivityIssues
}