using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class ReopenConversationCommand : IRequest<Unit>
{
    public Guid ConversationId { get; set; }
}

public class ReopenConversationCommandHandler : IRequestHandler<ReopenConversationCommand, Unit>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public ReopenConversationCommandHandler(
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

    public async ValueTask<Result<Unit>> Handle(ReopenConversationCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id.ToString();

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<Unit>("Conversation not found", "NotFound");
        }

        conversation.Reopen(_userContext.Id);
        conversation.SetModificationTracking(currentUserId, ipAddress);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
