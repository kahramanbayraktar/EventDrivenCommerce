using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Mappings
{
    public class InventoryItemMappingProfile : Profile
    {
        public InventoryItemMappingProfile()
        {
            CreateMap<InventoryItem, InventoryItemDTO>().ReverseMap();
        }
    }
}
