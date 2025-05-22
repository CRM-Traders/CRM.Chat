using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Application.Features.Conversations.DTOs;

public class ConversationDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ConversationType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ConversationMemberDto> Members { get; set; } = new();
}