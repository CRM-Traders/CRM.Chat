using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Domain.Entities.Messages;
using CRM.Chat.Domain.Entities.Messages.Enums;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Messages.Commands;

public class SendMessageCommand : IRequest<Guid>
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public List<Guid> AttachmentIds { get; set; } = new();
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IRepository<Message> _messageRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IRepository<Conversation> conversationRepository,
        IRepository<Message> messageRepository,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Guid>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<Guid>("Conversation not found", "NotFound");
        }

        if (!conversation.IsActive)
        {
            return Result.Failure<Guid>("Cannot send message to inactive conversation", "BadRequest");
        }

        // Get recipient IDs from conversation
        var recipientIds = conversation.GetMemberIds();

        var message = Message.Create(
            request.ConversationId,
            _userContext.Id,
            request.Content,
            request.Type,
            request.AttachmentIds,
            recipientIds,
            ipAddress);

        await _messageRepository.AddAsync(message, cancellationToken);
        conversation.AddMessage(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }
}