namespace CRM.Chat.Application.Features.Conversations.DTOs;

public class ConversationMemberDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
}