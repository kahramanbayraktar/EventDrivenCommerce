namespace ProductService.Domain.Events
{
    public class ProductUpdatedEvent
    {
        public Guid ProductId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
