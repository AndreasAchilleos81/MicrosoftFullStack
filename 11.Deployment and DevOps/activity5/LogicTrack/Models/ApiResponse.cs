namespace LogicTrack.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public object? Error { get; set; }
        public double ExecutionTimeMs { get; set; }

        public ApiResponse() { }

        public static ApiResponse<T> FromData(T data, double ms) => new ApiResponse<T> { Data = data, ExecutionTimeMs = ms };
        public static ApiResponse<T> FromError(object error, double ms) => new ApiResponse<T> { Error = error, ExecutionTimeMs = ms };
    }
}
