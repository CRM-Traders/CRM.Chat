namespace CRM.Chat.Application.Features.Conversations.DTOs;

public class MessageBriefDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
}