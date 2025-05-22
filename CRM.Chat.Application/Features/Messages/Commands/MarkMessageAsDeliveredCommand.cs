using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Persistence;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Messages;

namespace CRM.Chat.Application.Features.Messages.Commands;

public class MarkMessageAsDeliveredCommand : IRequest<Unit>
{
    public Guid MessageId { get; set; }
}

public class MarkMessageAsDeliveredCommandHandler : IRequestHandler<MarkMessageAsDeliveredCommand, Unit>
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public MarkMessageAsDeliveredCommandHandler(
        IRepository<Message> messageRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result<Unit>> Handle(MarkMessageAsDeliveredCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
        if (message == null)
        {
            return Result.Failure<Unit>("Message not found", "NotFound");
        }

        message.MarkAsDeliveredTo(_userContext.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}