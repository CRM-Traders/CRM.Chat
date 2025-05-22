using CRM.Chat.Domain.Common.Entities;

namespace CRM.Chat.Domain.Entities.MessageStatuses;

public class MessageStatus : AuditableEntity
{
    public Guid MessageId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsDelivered { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    private MessageStatus() { }

    public MessageStatus(Guid messageId, Guid userId)
    {
        MessageId = messageId;
        UserId = userId;
        IsDelivered = false;
        IsRead = false;
    }

    public void MarkAsDelivered()
    {
        if (!IsDelivered)
        {
            IsDelivered = true;
            DeliveredAt = DateTimeOffset.UtcNow;
        }
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            // If somehow not marked as delivered yet, mark it now
            if (!IsDelivered)
            {
                IsDelivered = true;
                DeliveredAt = DateTimeOffset.UtcNow;
            }

            IsRead = true;
            ReadAt = DateTimeOffset.UtcNow;
        }
    }
}