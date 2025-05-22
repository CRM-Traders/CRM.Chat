using CRM.Chat.Domain.Entities.OutboxMessages;

namespace CRM.Chat.Domain.Common.Events;

public interface IExternalEventPublisher
{
    Task PublishEventAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
}