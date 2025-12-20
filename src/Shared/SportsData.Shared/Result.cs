using System.Text.Json.Serialization;

namespace SportsData.Shared
{
    // Non-generic result for void operations
    public class Result 
    {
        public bool IsSuccess { get; }
        public string[] Errors { get; }

        protected Result(bool isSuccess, string[] errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? Array.Empty<string>();
        }

        public static Result Success() => new(true, null);
        public static Result Failure(params string[] errors) => new(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, string[] errors) : base(isSuccess, errors)
        {
            Value = value;
        }

        public static new Result<T> Success(T value) => new(true, value, null);
        public static new Result<T> Failure(params string[] errors) => new(false, default, errors);
    }
}
