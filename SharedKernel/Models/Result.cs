namespace SharedKernel.Models
{
    public class Result<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = null!;

        public static Result<T> SuccessResult(T data, string message = "")
        {
            return new Result<T> { Data = data, Success = true, Message = message };
        }

        public static Result<T> FailureResult(string message = "", Exception exception = null!)
        {
            return new Result<T> { Success = false, Message = message };
        }
    }
}
