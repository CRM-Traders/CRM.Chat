using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.UserPresences.Enums;

namespace CRM.Chat.Domain.Entities.UserPresences;

public class UserPresence : Entity
{
    public Guid UserId { get; private set; }
    public PresenceStatus Status { get; private set; }
    public DateTimeOffset LastActive { get; private set; }

    private UserPresence() { }

    public static UserPresence Create(Guid userId)
    {
        return new UserPresence
        {
            UserId = userId,
            Status = PresenceStatus.Offline,
            LastActive = DateTimeOffset.UtcNow
        };
    }

    public void SetOnline()
    {
        Status = PresenceStatus.Online;
        LastActive = DateTimeOffset.UtcNow;
    }

    public void SetAway()
    {
        Status = PresenceStatus.Away;
        LastActive = DateTimeOffset.UtcNow;
    }

    public void SetBusy()
    {
        Status = PresenceStatus.Busy;
        LastActive = DateTimeOffset.UtcNow;
    }

    public void SetOffline()
    {
        Status = PresenceStatus.Offline;
        LastActive = DateTimeOffset.UtcNow;
    }

    public void UpdateLastActive()
    {
        LastActive = DateTimeOffset.UtcNow;

        // If user was away but is now active, set them back to online
        if (Status == PresenceStatus.Away)
        {
            Status = PresenceStatus.Online;
        }
    }
}