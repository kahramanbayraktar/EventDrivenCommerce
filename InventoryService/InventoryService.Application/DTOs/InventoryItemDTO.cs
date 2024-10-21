namespace InventoryService.Application.DTOs
{
    public class InventoryItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
