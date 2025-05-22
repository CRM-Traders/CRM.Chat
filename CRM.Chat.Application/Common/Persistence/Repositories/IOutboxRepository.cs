using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Domain.Entities.OutboxMessages;

namespace CRM.Chat.Application.Common.Persistence.Repositories;

public interface IOutboxRepository : IRepository<OutboxMessage>
{
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(
        int maxMessages,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesForPartitionAsync(
        int partitionId,
        int partitionCount,
        int maxMessages,
        CancellationToken cancellationToken = default);

    Task<bool> TryClaimMessageAsync(
        Guid messageId,
        string instanceId,
        CancellationToken cancellationToken = default);
}