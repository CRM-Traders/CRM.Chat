using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Application.Features.Conversations.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ConversationType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ConversationMemberDto> Members { get; set; } = new();
    public MessageBriefDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}