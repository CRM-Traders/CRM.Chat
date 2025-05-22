using CRM.Chat.Application.Common.Abstractions.Specifications;
using CRM.Chat.Domain.Entities.Messages;

namespace CRM.Chat.Application.Features.Messages.Specifications;

public class MessagesForConversationSpecification : BaseSpecification<Message>
{
    public MessagesForConversationSpecification(
        Guid conversationId,
        DateTimeOffset? before = null,
        int limit = 50)
        : base(x => x.ConversationId == conversationId)
    {
        if (before.HasValue)
        {
            AddCriteria(x => x.SentAt < before.Value);
        }

        AddInclude(x => x.Statuses);
        ApplyOrderByDescending(x => x.SentAt);
        ApplyPaging(0, limit);
    }
}