using CRM.Chat.Domain.Entities.Messages.Enums;

namespace CRM.Chat.Application.Features.Messages.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; }
    public bool IsEdited { get; set; }
    public MessageType Type { get; set; }
    public List<Guid> AttachmentIds { get; set; } = new();
}