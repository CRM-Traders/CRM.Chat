namespace CRM.Chat.Domain.Entities.Conversations.Enums;

public enum ConversationType
{
    External = 1, // User to Operator
    Direct = 2,   // Internal user to internal user
    Group = 3,    // Group of internal users
}