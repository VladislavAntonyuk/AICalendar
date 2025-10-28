using System.Diagnostics.CodeAnalysis;

namespace AICalendar.Client.Infrastructure.OperationResult;

public record OperationResult<T>
{
	public T? Value { get; private init; }

	public OperationResultError? Error { get; private init; }

	[MemberNotNullWhen(true, nameof(Value))]
	[MemberNotNullWhen(false, nameof(Error))]
	public bool IsSuccessful => Error is null;

	public static OperationResult<T> Success(T value)
	{
		return new OperationResult<T>
		{
			Value = value
		};
	}

	public static OperationResult<T> Failed(string error, OperationResultErrorType errorType = OperationResultErrorType.Unknown)
	{
		return new OperationResult<T>()
		{
			Error = new OperationResultError
			{
				Message = error,
				Type = errorType
			}
		};
	}
}