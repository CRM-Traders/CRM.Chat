using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;

namespace CRM.Chat.Application.Features.Conversations.Commands;

public class SendTypingIndicatorCommand : IRequest<Unit>
{
    public Guid ConversationId { get; set; }
    public bool IsTyping { get; set; }
}

public class SendTypingIndicatorCommandHandler : IRequestHandler<SendTypingIndicatorCommand, Unit>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public SendTypingIndicatorCommandHandler(
        IRepository<Conversation> conversationRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(SendTypingIndicatorCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            return Result.Failure<Unit>("Conversation not found", "NotFound");
        }

        conversation.EmitTypingIndicator(_userContext.Id, request.IsTyping);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}