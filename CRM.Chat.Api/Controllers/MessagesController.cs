using CRM.Chat.Api.Controllers.Base;
using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Features.Messages.Commands;
using CRM.Chat.Application.Features.Messages.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Chat.Api.Controllers;

public class MessagesController : BaseController
{
    public MessagesController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Send a message in a conversation
    /// Request body example:
    /// {
    ///   "content": "Hello, this is a message",
    ///   "type": 1,
    ///   "attachmentIds": ["3fa85f64-5717-4562-b3fc-2c963f66afa6", "4fa85f64-5717-4562-b3fc-2c963f66afa7"]
    /// }
    /// </summary>
    [HttpPost("{conversationId:guid}")]
    public async Task<IResult> SendMessage(Guid conversationId, SendMessageCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = conversationId;
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Get messages in a conversation
    /// </summary>
    [HttpGet("{conversationId:guid}")]
    public async Task<IResult> GetMessages(Guid conversationId, [FromQuery] GetMessagesQuery query, CancellationToken cancellationToken)
    {
        query.ConversationId = conversationId;
        return await SendAsync(query, cancellationToken);
    }

    /// <summary>
    /// Edit a message
    /// </summary>
    [HttpPut("{conversationId:guid}")]
    public async Task<IResult> EditMessage(Guid conversationId, Guid messageId, EditMessageCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = conversationId;
        command.MessageId = messageId;
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    [HttpPost("{messageId:guid}/read")]
    public async Task<IResult> MarkAsRead(Guid messageId, CancellationToken cancellationToken)
    {
        var command = new MarkMessageAsReadCommand { MessageId = messageId };
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Mark a message as delivered
    /// </summary>
    [HttpPost("{messageId:guid}/delivered")]
    public async Task<IResult> MarkAsDelivered(Guid messageId, CancellationToken cancellationToken)
    {
        var command = new MarkMessageAsDeliveredCommand { MessageId = messageId };
        return await SendAsync(command, cancellationToken);
    }
}