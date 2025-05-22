using CRM.Chat.Application.Common.Abstractions.Mediators;
using CRM.Chat.Application.Common.Abstractions.Users;
using CRM.Chat.Application.Common.Services.Identity;
using CRM.Chat.Application.Features.Conversations.Specifications;
using CRM.Chat.Domain.Common.Models;
using CRM.Chat.Domain.Entities.Conversations.Enums;
using CRM.Chat.Domain.Entities.Conversations;
using CRM.Chat.Application.Features.Conversations.DTOs;

namespace CRM.Chat.Application.Features.Conversations.Queries;

public class GetConversationsQuery : IRequest<List<ConversationDto>>
{
    public ConversationType? Type { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;

    public GetConversationsQueryHandler(
        IRepository<Conversation> conversationRepository,
        IUserContext userContext,
        IIdentityService identityService)
    {
        _conversationRepository = conversationRepository;
        _userContext = userContext;
        _identityService = identityService;
    }

    public async ValueTask<Result<List<ConversationDto>>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var spec = new UserConversationsSpecification(
            _userContext.Id,
            request.Type,
            request.IsActive,
            request.PageNumber,
            request.PageSize);

        var conversations = await _conversationRepository.ListAsync(spec, cancellationToken);

        var userIds = conversations
            .SelectMany(c => c.Members)
            .Select(m => m.UserId)
            .Distinct()
            .ToList();

        var users = await _identityService.GetUsersAsync(userIds);
        var userDict = users.ToDictionary(u => u.Id, u => u);

        var conversationDtos = conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            Members = c.Members.Select(m => new ConversationMemberDto
            {
                UserId = m.UserId,
                IsAdmin = m.IsAdmin,
                UserName = userDict.TryGetValue(m.UserId, out var user) ? user.FullName : "Unknown User",
                JoinedAt = m.JoinedAt,
                LastSeen = m.LastSeen
            }).ToList(),
            LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault() != null
                ? new MessageBriefDto
                {
                    Id = c.Messages.OrderByDescending(m => m.SentAt).First().Id,
                    Content = c.Messages.OrderByDescending(m => m.SentAt).First().Content,
                    SentAt = c.Messages.OrderByDescending(m => m.SentAt).First().SentAt,
                    SenderId = c.Messages.OrderByDescending(m => m.SentAt).First().SenderId,
                    SenderName = userDict.TryGetValue(c.Messages.OrderByDescending(m => m.SentAt).First().SenderId, out var sender)
                        ? sender.FullName : "Unknown User"
                }
                : null
        }).ToList();

        return Result.Success(conversationDtos);
    }
}