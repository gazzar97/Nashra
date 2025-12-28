using System.Text.Json.Serialization;

namespace SportsData.Shared
{
    // Error helper class for creating typed errors
    public static class Error
    {
        public static ErrorDetail NotFound(string code, string message) => 
            new ErrorDetail(code, message, "NotFound");

        public static ErrorDetail Validation(string code, string message) => 
            new ErrorDetail(code, message, "Validation");

        public static ErrorDetail Conflict(string code, string message) => 
            new ErrorDetail(code, message, "Conflict");

        public static ErrorDetail Failure(string code, string message) => 
            new ErrorDetail(code, message, "Failure");
    }

    public record ErrorDetail(string Code, string Message, string Type);

    // Non-generic result for void operations
    public class Result 
    {
        public bool IsSuccess { get; }
        public string[] Errors { get; }
        public int StatusCode { get; }

        protected Result(bool isSuccess, string[] errors, int statusCode = 200)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? Array.Empty<string>();
            StatusCode = statusCode;
        }

        public static Result Success(int statusCode = 204) => new(true, null, statusCode);
        public static Result Failure(params string[] errors) => new(false, errors, 400);
        public static Result Failure(ErrorDetail error, int statusCode) => 
            new(false, new[] { $"{error.Code}: {error.Message}" }, statusCode);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, string[] errors, int statusCode = 200) : base(isSuccess, errors, statusCode)
        {
            Value = value;
        }

        public static new Result<T> Success(T value, int statusCode = 200) => new(true, value, null, statusCode);
        public static new Result<T> Failure(params string[] errors) => new(false, default, errors, 400);
        public static Result<T> Failure(ErrorDetail error, int statusCode) => 
            new(false, default, new[] { $"{error.Code}: {error.Message}" }, statusCode);
    }
}
