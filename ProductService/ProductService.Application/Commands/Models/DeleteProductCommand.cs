using MediatR;
using SharedKernel.Models;

namespace ProductService.Application.Commands.Models
{
    public class DeleteProductCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}
