using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using SharedKernel.Models;

namespace InventoryService.Application.Interfaces
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _repository;
        private readonly IMapper _mapper;

        public InventoryItemService(IInventoryItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<InventoryItemDTO>> CreateInventoryItem(InventoryItemDTO itemDTO)
        {
            var item = _mapper.Map<InventoryItem>(itemDTO);

            var itemCreated = await _repository.CreateAsync(item);

            var itemCreatedDto = _mapper.Map<InventoryItemDTO>(itemCreated);

            return Result<InventoryItemDTO>.SuccessResult(itemCreatedDto);
        }
    }
}
