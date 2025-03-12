namespace ReadOn.Shared
{
    public class ApiResponse<A>
    {
        public bool Success {get; set;}
        public string Message { get; set;}
        public A Data { get; set;}
        public int StatusCode {get; set;}

        public ApiResponse(bool success, string message, A data, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }
    }
}
