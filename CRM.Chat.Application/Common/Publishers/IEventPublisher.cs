using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Application.Common.Publishers;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}