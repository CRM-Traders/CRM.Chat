using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Application.Common.Services.Identity;
using CRM.Chat.Application.Features.Messages.Specifications;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations.Enums;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Domain.Entities.Messages;
using CRM.Chat.Application.Features.Messages.DTOs;

namespace CRM.Chat.Application.Features.Messages.Queries;

public class GetMessagesQuery : IRequest<List<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public DateTimeOffset? Before { get; set; }
    public int Limit { get; set; } = 50;
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesQueryHandler(
        IRepository<Message> messageRepository,
        IRepository<Conversation> conversationRepository,
        IUserContext userContext,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _userContext = userContext;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<List<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<List<MessageDto>>("Conversation not found", "NotFound");
        }

        if (conversation.Type != ConversationType.External &&
            !conversation.Members.Any(m => m.UserId == _userContext.Id && !m.IsDeleted))
        {
            return Result.Failure<List<MessageDto>>("Access denied to this conversation", "Forbidden");
        }

        var spec = new MessagesForConversationSpecification(
            request.ConversationId,
            request.Before,
            request.Limit);

        var messages = await _messageRepository.ListAsync(spec, cancellationToken);

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var users = await _identityService.GetUsersAsync(senderIds);
        var userDict = users.ToDictionary(u => u.Id, u => u);

        foreach (var message in messages.Where(m => m.SenderId != _userContext.Id))
        {
            message.MarkAsReadBy(_userContext.Id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var messageDtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            SenderName = userDict.TryGetValue(m.SenderId, out var user) ? user.FullName : "Unknown User",
            Content = m.Content,
            SentAt = m.SentAt,
            IsEdited = m.IsEdited,
            Type = m.Type,
            AttachmentIds = m.AttachmentIds
        }).ToList();

        return Result.Success(messageDtos);
    }
}