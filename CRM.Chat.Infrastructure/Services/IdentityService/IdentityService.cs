using CRM.Chat.Application.Common.DTOs;
using CRM.Chat.Application.Common.Services.Identity;

namespace CRM.Chat.Infrastructure.Services.IdentityService;

public class IdentityService : IIdentityService
{
    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        // TODO: Implement integration with CRM.Identity microservice
        await Task.Delay(1);
        return new UserDto
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com",
            IsActive = true
        };
    }

    public async Task<List<UserDto>> GetUsersAsync(IEnumerable<Guid> userIds)
    {
        // TODO: Implement integration with CRM.Identity microservice
        await Task.Delay(1);
        return userIds.Select(id => new UserDto
        {
            Id = id,
            FullName = "Test User",
            Email = "test@example.com",
            IsActive = true
        }).ToList();
    }

    public async Task<List<UserDto>> SearchUsersAsync(string query, int limit = 10)
    {
        // TODO: Implement integration with CRM.Identity microservice
        await Task.Delay(1);
        return new List<UserDto>();
    }

    public async Task<List<UserDto>> GetOperatorsAsync(bool onlineOnly = false)
    {
        // TODO: Implement integration with CRM.Identity microservice
        await Task.Delay(1);
        return new List<UserDto>();
    }
}