using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.UserPresences.Enums;

namespace CRM.Chat.Domain.Entities.UserPresences.DomainEvents;

public class PresenceChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public PresenceStatus OldStatus { get; }
    public PresenceStatus NewStatus { get; }
    public DateTimeOffset LastActive { get; }
    public string? StatusMessage { get; }

    public PresenceChangedEvent(Guid presenceId, Guid userId,
        PresenceStatus oldStatus, PresenceStatus newStatus,
        DateTimeOffset lastActive, string? statusMessage = null)
        : base(presenceId, nameof(UserPresence))
    {
        UserId = userId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        LastActive = lastActive;
        StatusMessage = statusMessage;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}