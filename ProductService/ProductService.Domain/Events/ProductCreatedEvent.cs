﻿namespace ProductService.Domain.Events
{
    public class ProductCreatedEvent
    {
        public Guid ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
