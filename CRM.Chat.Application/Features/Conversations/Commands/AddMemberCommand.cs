using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class AddMemberCommand : IRequest<Unit>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }
}

public class AddMemberCommandHandler : IRequestHandler<AddMemberCommand, Unit>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public AddMemberCommandHandler(
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

    public async ValueTask<Result<Unit>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id.ToString();

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<Unit>("Conversation not found", "NotFound");
        }

        if (!conversation.Members.Any(m => m.UserId == _userContext.Id && m.IsAdmin && !m.IsDeleted))
        {
            return Result.Failure<Unit>("Only admins can add members to a conversation", "Forbidden");
        }

        conversation.AddMember(request.UserId, request.IsAdmin);
        conversation.SetModificationTracking(currentUserId, ipAddress);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}