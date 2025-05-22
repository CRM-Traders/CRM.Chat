using CRM.Chat.Api.Controllers.Base;
using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Features.Conversations.Commands;
using CRM.Chat.Application.Features.Conversations.Queries;
using CRM.Chat.Application.Features.Messages.Commands;
using CRM.Chat.Application.Features.Messages.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Chat.Api.Controllers;

public class ConversationsController : BaseController
{
    public ConversationsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Create an external conversation between a customer and operators
    /// </summary>
    [HttpPost("external")]
    public async Task<IResult> CreateExternalConversation(CreateExternalConversationCommand command, CancellationToken cancellationToken)
    {
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Create a direct conversation between two internal users
    /// </summary>
    [HttpPost("direct")]
    public async Task<IResult> CreateDirectConversation(CreateDirectConversationCommand command, CancellationToken cancellationToken)
    {
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Create a group conversation with multiple members
    /// </summary>
    [HttpPost("group")]
    public async Task<IResult> CreateGroupConversation(CreateGroupConversationCommand command, CancellationToken cancellationToken)
    {
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Get all conversations for the current user
    /// </summary>
    [HttpGet]
    public async Task<IResult> GetConversations([FromQuery] GetConversationsQuery query, CancellationToken cancellationToken)
    {
        return await SendAsync(query, cancellationToken);
    }

    /// <summary>
    /// Get details of a specific conversation
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IResult> GetConversationDetails(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetConversationDetailsQuery { ConversationId = id };
        return await SendAsync(query, cancellationToken);
    }

    /// <summary>
    /// Close a conversation
    /// </summary>
    [HttpPost("{id:guid}/close")]
    public async Task<IResult> CloseConversation(Guid id, CancellationToken cancellationToken)
    {
        var command = new CloseConversationCommand { ConversationId = id };
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Send a message in a conversation
    /// Request body example:
    /// {
    ///   "content": "Hello, this is a message",
    ///   "type": 1,
    ///   "attachmentIds": ["3fa85f64-5717-4562-b3fc-2c963f66afa6", "4fa85f64-5717-4562-b3fc-2c963f66afa7"]
    /// }
    /// </summary>
    [HttpPost("{id:guid}/messages")]
    public async Task<IResult> SendMessage(Guid id, SendMessageCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = id;
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Get messages in a conversation
    /// </summary>
    [HttpGet("{id:guid}/messages")]
    public async Task<IResult> GetMessages(Guid id, [FromQuery] GetMessagesQuery query, CancellationToken cancellationToken)
    {
        query.ConversationId = id;
        return await SendAsync(query, cancellationToken);
    }

    /// <summary>
    /// Edit a message
    /// </summary>
    [HttpPut("{id:guid}/messages/{messageId:guid}")]
    public async Task<IResult> EditMessage(Guid id, Guid messageId, EditMessageCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = id;
        command.MessageId = messageId;
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Add a member to a group conversation
    /// </summary>
    [HttpPost("{id:guid}/members")]
    public async Task<IResult> AddMember(Guid id, AddMemberCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = id;
        return await SendAsync(command, cancellationToken);
    }
}