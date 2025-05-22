using CRM.Chat.Application.Common.Abstractions.Specifications;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Application.Features.Conversations.Specifications;

public class DirectConversationBetweenUsersSpecification : BaseSpecification<Conversation>
{
    public DirectConversationBetweenUsersSpecification(Guid userId1, Guid userId2)
        : base(x => x.Type == ConversationType.Direct &&
                   x.Members.Count(m => (m.UserId == userId1 || m.UserId == userId2) && !m.IsDeleted) == 2 &&
                   x.Members.Any(m => m.UserId == userId1 && !m.IsDeleted) &&
                   x.Members.Any(m => m.UserId == userId2 && !m.IsDeleted))
    {
        AddInclude(x => x.Members);
    }
}