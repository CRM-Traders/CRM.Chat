using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class CreateExternalConversationCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
}

public class CreateExternalConversationCommandHandler : IRequestHandler<CreateExternalConversationCommand, Guid>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExternalConversationCommandHandler(
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

    public async ValueTask<Result<Guid>> Handle(CreateExternalConversationCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id;

        var conversation = Conversation.CreateExternalConversation(request.UserId, currentUserId);
        conversation.SetCreationTracking(currentUserId.ToString(), ipAddress);

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(conversation.Id);
    }
}