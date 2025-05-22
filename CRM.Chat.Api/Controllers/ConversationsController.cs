using CRM.Chat.Api.Controllers.Base;
using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Features.Conversations.Commands;
using CRM.Chat.Application.Features.Conversations.Queries;
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
    [HttpGet("{conversationId:guid}")]
    public async Task<IResult> GetConversationDetails(Guid conversationId, CancellationToken cancellationToken)
    {
        var query = new GetConversationDetailsQuery { ConversationId = conversationId };
        return await SendAsync(query, cancellationToken);
    }

    /// <summary>
    /// Add a member to a group conversation
    /// </summary>
    [HttpPost("{conversationId:guid}/members")]
    public async Task<IResult> AddMember(Guid conversationId, AddMemberCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = conversationId;
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Close a conversation
    /// </summary>
    [HttpPost("{conversationId:guid}/close")]
    public async Task<IResult> CloseConversation(Guid conversationId, CancellationToken cancellationToken)
    {
        var command = new CloseConversationCommand { ConversationId = conversationId };
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Reopen a closed conversation
    /// </summary>
    [HttpPost("{conversationId:guid}/reopen")]
    public async Task<IResult> ReopenConversation(Guid conversationId, CancellationToken cancellationToken)
    {
        var command = new ReopenConversationCommand { ConversationId = conversationId };
        return await SendAsync(command, cancellationToken);
    }

    /// <summary>
    /// Send typing indicator in a conversation
    /// </summary>
    [HttpPost("{conversationId:guid}/typing")]
    public async Task<IResult> SendTypingIndicator(Guid conversationId, SendTypingIndicatorCommand command, CancellationToken cancellationToken)
    {
        command.ConversationId = conversationId;
        return await SendAsync(command, cancellationToken);
    }
}