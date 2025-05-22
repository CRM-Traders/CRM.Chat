using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations.Enums;
using CRM.Chat.Domain.Entities.Conversations;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class CloseConversationCommand : IRequest<Unit>
{
    public Guid ConversationId { get; set; }
}

public class CloseConversationCommandHandler : IRequestHandler<CloseConversationCommand, Unit>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CloseConversationCommandHandler(
        IRepository<Conversation> conversationRepository,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(CloseConversationCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id.ToString();

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<Unit>("Conversation not found", "NotFound");
        }

        if (conversation.Type == ConversationType.Group &&
            !conversation.Members.Any(m => m.UserId == _userContext.Id && m.IsAdmin && !m.IsDeleted))
        {
            return Result.Failure<Unit>("Only admins can close group conversations", "Forbidden");
        }

        conversation.Close(_userContext.Id);
        conversation.SetModificationTracking(currentUserId, ipAddress);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}