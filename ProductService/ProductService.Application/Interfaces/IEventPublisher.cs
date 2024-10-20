namespace ProductService.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string topic, TEvent eventMessage);
    }
}
