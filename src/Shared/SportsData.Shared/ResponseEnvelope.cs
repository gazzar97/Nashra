namespace SportsData.Shared
{
    public class ResponseEnvelope<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string[] Errors { get; }
        public DateTime Timestamp { get; }

        public ResponseEnvelope(T? data, string[] errors, bool isSuccess = true)
        {
            Data = data;
            Errors = errors ?? Array.Empty<string>();
            IsSuccess = isSuccess;
            Timestamp = DateTime.UtcNow;
        }

        public static ResponseEnvelope<T> Success(T data) 
            => new(data, Array.Empty<string>(), true);

        public static ResponseEnvelope<T> Failure(string[] errors) 
            => new(default, errors, false);
    }
}
