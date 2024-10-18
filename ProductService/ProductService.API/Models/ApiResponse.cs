namespace ProductService.API.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public DateTime ResponseCreated { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; } = null!;
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }
}
