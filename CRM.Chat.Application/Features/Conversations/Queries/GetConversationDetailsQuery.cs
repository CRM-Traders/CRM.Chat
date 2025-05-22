using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Services.Identity;
using CRM.Chat.Application.Features.Conversations.DTOs;
using CRM.Chat.Application.Features.Conversations.Specifications;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Application.Features.Conversations.Queries;

public class GetConversationDetailsQuery : IRequest<ConversationDetailsDto>
{
    public Guid ConversationId { get; set; }
}

public class GetConversationDetailsQueryHandler : IRequestHandler<GetConversationDetailsQuery, ConversationDetailsDto>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;

    public GetConversationDetailsQueryHandler(
        IRepository<Conversation> conversationRepository,
        IUserContext userContext,
        IIdentityService identityService)
    {
        _conversationRepository = conversationRepository;
        _userContext = userContext;
        _identityService = identityService;
    }

    public async ValueTask<Result<ConversationDetailsDto>> Handle(GetConversationDetailsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationWithMembersSpecification(request.ConversationId);
        var conversation = await _conversationRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (conversation == null)
        {
            return Result.Failure<ConversationDetailsDto>("Conversation not found", "NotFound");
        }

        if (conversation.Type != ConversationType.External &&
            !conversation.Members.Any(m => m.UserId == _userContext.Id && !m.IsDeleted))
        {
            return Result.Failure<ConversationDetailsDto>("Access denied to this conversation", "Forbidden");
        }

        var userIds = conversation.Members.Select(m => m.UserId).Distinct().ToList();
        var users = await _identityService.GetUsersAsync(userIds);
        var userDict = users.ToDictionary(u => u.Id, u => u);

        var conversationDetails = new ConversationDetailsDto
        {
            Id = conversation.Id,
            Name = conversation.Name,
            Type = conversation.Type,
            IsActive = conversation.IsActive,
            CreatedAt = conversation.CreatedAt,
            Members = conversation.Members.Select(m => new ConversationMemberDto
            {
                UserId = m.UserId,
                IsAdmin = m.IsAdmin,
                UserName = userDict.TryGetValue(m.UserId, out var user) ? user.FullName : "Unknown User",
                JoinedAt = m.JoinedAt,
                LastSeen = m.LastSeen
            }).ToList()
        };

        return Result.Success(conversationDetails);
    }
}