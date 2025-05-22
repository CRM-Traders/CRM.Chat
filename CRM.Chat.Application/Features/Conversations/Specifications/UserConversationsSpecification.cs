using CRM.Chat.Application.Common.Abstractions.Specifications;
using CRM.Chat.Domain.Entities.Conversations.Enums;
using CRM.Chat.Domain.Entities.Conversations;

namespace CRM.Chat.Application.Features.Conversations.Specifications;

public class UserConversationsSpecification : BaseSpecification<Conversation>
{
    public UserConversationsSpecification(
        Guid userId,
        ConversationType? type = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 20)
        : base(x => (x.Type == ConversationType.External && x.ExternalUserId == userId) ||
                    (x.Type != ConversationType.External && x.Members.Any(m => m.UserId == userId && !m.IsDeleted)))
    {
        if (type.HasValue)
        {
            AddCriteria(x => x.Type == type.Value);
        }

        if (isActive.HasValue)
        {
            AddCriteria(x => x.IsActive == isActive.Value);
        }

        AddInclude(x => x.Members);
        AddInclude(x => x.Messages);
        ApplyOrderByDescending(x => x.CreatedAt);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}

public class ConversationWithMembersSpecification : BaseSpecification<Conversation>
{
    public ConversationWithMembersSpecification(Guid conversationId)
        : base(x => x.Id == conversationId)
    {
        AddInclude(x => x.Members);
    }
}