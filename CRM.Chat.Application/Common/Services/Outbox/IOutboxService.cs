﻿using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.OutboxMessages;

namespace CRM.Chat.Application.Common.Services.Outbox;

public interface IOutboxService
{
    Task<IReadOnlyDictionary<Guid, OutboxMessage>> CreateOutboxMessagesAsync(
            IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default);

    Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default);
    Task ProcessOutboxMessagesForPartitionAsync(
            int partitionId,
            int partitionCount,
            CancellationToken cancellationToken = default);
}