using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Messages.DomainEvents;

namespace CRM.Chat.Infrastructure.EventHandler.Messages;

public class MessageSentEventHandler : IDomainEventHandler<MessageSentEvent>
{
   

    public async Task HandleAsync(MessageSentEvent domainEvent, CancellationToken cancellationToken = default)
    {
        
    }
}