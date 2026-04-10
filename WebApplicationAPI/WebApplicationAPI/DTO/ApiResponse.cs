namespace WebApplicationAPI.DTO
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                Status = "success",
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message, T data = default)
        {
            return new ApiResponse<T>
            {
                Status = "fail",
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Error(string message, T data = default)
        {
            return new ApiResponse<T>
            {
                Status = "error",
                Message = message,
                Data = data
            };
        }
    }

    public class ApiResponse
    {
        public string Status { get; set; }
        public string? Message { get; set; }

        public static ApiResponse Success(string message = null)
        {
            return new ApiResponse
            {
                Status = "success",
                Message = message
            };
        }

        public static ApiResponse Fail(string message)
        {
            return new ApiResponse
            {
                Status = "fail",
                Message = message
            };
        }

        public static ApiResponse Error(string message)
        {
            return new ApiResponse
            {
                Status = "error",
                Message = message
            };
        }
    }
}
