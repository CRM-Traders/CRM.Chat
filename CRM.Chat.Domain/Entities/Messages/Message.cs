using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.Messages.DomainEvents;
using CRM.Chat.Domain.Entities.Messages.Enums;
using CRM.Chat.Domain.Entities.MessageStatuses;

namespace CRM.Chat.Domain.Entities.Messages;

public class Message : AggregateRoot
{
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public bool IsEdited { get; private set; }
    public MessageType Type { get; private set; }
    public List<Guid> AttachmentIds { get; private set; } = new();
    private readonly List<MessageStatus> _statuses = new();
    public IReadOnlyCollection<MessageStatus> Statuses => _statuses.AsReadOnly();

    private Message() { }

    public static Message Create(Guid conversationId, Guid senderId, string content,
        MessageType type = MessageType.Text, List<Guid>? attachmentIds = null,
        ICollection<Guid>? recipientIds = null, string? senderIp = null)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTimeOffset.UtcNow,
            IsEdited = false,
            Type = type,
            AttachmentIds = attachmentIds ?? new List<Guid>()
        };

        message.SetCreationTracking(senderId.ToString(), senderIp);

        // Create status records for all recipients
        if (recipientIds != null)
        {
            foreach (var recipientId in recipientIds.Where(id => id != senderId))
            {
                var status = new MessageStatus(message.Id, recipientId);
                message._statuses.Add(status);
            }
        }

        message.AddDomainEvent(new MessageSentEvent(
            message.Id,
            conversationId,
            senderId,
            content,
            type,
            message.AttachmentIds,
            recipientIds ?? new List<Guid>()));

        return message;
    }

    public void Edit(string newContent, Guid editedBy, string? ipAddress, ICollection<Guid>? recipientIds = null)
    {
        if (SenderId != editedBy)
            throw new InvalidOperationException("Only the sender can edit their message");

        if (Content == newContent)
            return;

        var oldContent = Content;
        Content = newContent;
        IsEdited = true;
        SetModificationTracking(editedBy.ToString(), ipAddress);

        AddDomainEvent(new MessageEditedEvent(
            Id,
            ConversationId,
            SenderId,
            oldContent,
            newContent,
            recipientIds ?? new List<Guid>()));
    }

    public void Delete(Guid deletedBy, string? ipAddress, ICollection<Guid>? recipientIds = null)
    {
        if (SenderId != deletedBy)
            throw new InvalidOperationException("Only the sender can delete their message");

        SetDeletionTracking(deletedBy.ToString(), ipAddress);

        AddDomainEvent(new MessageDeletedEvent(
            Id,
            ConversationId,
            SenderId,
            deletedBy,
            Content,
            recipientIds ?? new List<Guid>()));
    }

    public void MarkAsDeliveredTo(Guid userId)
    {
        var status = _statuses.FirstOrDefault(s => s.UserId == userId);
        if (status == null)
        {
            status = new MessageStatus(Id, userId);
            _statuses.Add(status);
        }

        if (!status.IsDelivered)
        {
            status.MarkAsDelivered();
            AddDomainEvent(new MessageDeliveredEvent(Id, ConversationId, SenderId, userId));
        }
    }

    public void MarkAsReadBy(Guid userId)
    {
        var status = _statuses.FirstOrDefault(s => s.UserId == userId);
        if (status == null)
        {
            status = new MessageStatus(Id, userId);
            _statuses.Add(status);
        }

        if (!status.IsRead)
        {
            status.MarkAsRead();
            AddDomainEvent(new MessageReadEvent(Id, ConversationId, SenderId, userId));
        }
    }

    public bool IsDeliveredTo(Guid userId)
    {
        return _statuses.Any(s => s.UserId == userId && s.IsDelivered);
    }

    public bool IsReadBy(Guid userId)
    {
        return _statuses.Any(s => s.UserId == userId && s.IsRead);
    }

    public int GetDeliveredCount()
    {
        return _statuses.Count(s => s.IsDelivered);
    }

    public int GetReadCount()
    {
        return _statuses.Count(s => s.IsRead);
    }
}