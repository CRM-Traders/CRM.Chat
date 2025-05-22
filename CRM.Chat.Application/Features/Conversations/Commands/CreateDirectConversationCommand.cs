using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Application.Features.Conversations.Specifications;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class CreateDirectConversationCommand : IRequest<Guid>
{
    public Guid RecipientId { get; set; }
}

public class CreateDirectConversationCommandHandler : IRequestHandler<CreateDirectConversationCommand, Guid>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDirectConversationCommandHandler(
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

    public async ValueTask<Result<Guid>> Handle(CreateDirectConversationCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id.ToString();

        // Check if conversation already exists
        var spec = new DirectConversationBetweenUsersSpecification(_userContext.Id, request.RecipientId);
        var existingConversation = await _conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (existingConversation != null)
        {
            return Result.Success(existingConversation.Id);
        }

        var conversation = Conversation.CreateDirectConversation(_userContext.Id, request.RecipientId);
        conversation.SetCreationTracking(currentUserId, ipAddress);

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(conversation.Id);
    }
}