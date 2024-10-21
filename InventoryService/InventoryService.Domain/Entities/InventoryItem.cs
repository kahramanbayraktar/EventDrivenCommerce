namespace InventoryService.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }             // Reference to Product entity
        public int Quantity { get; set; }               // Available stock quantity
        public DateTime LastUpdatedAt { get; set; }     // Timestamp for last stock update
    }
}
