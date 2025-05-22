using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Messages;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Features.Messages.Commands;

public class EditMessageCommand : IRequest<Unit>
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public EditMessageCommandHandler(
        IRepository<Message> messageRepository,
        IUserContext userContext,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var currentUserId = _userContext.Id.ToString();

        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
        if (message == null)
        {
            return Result.Failure<Unit>("Message not found", "NotFound");
        }

        if (message.ConversationId != request.ConversationId)
        {
            return Result.Failure<Unit>("Message does not belong to specified conversation", "BadRequest");
        }

        if (message.SenderId != _userContext.Id)
        {
            return Result.Failure<Unit>("You can only edit your own messages", "Forbidden");
        }

        message.Edit(request.Content, currentUserId, ipAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}