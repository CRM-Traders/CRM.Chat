using CRM.Chat.Application.Common.DTOs;

namespace CRM.Chat.Application.Common.Services.Identity;

public interface IIdentityService
{
    Task<UserDto?> GetUserAsync(Guid userId);
    Task<List<UserDto>> GetUsersAsync(IEnumerable<Guid> userIds);
    Task<List<UserDto>> SearchUsersAsync(string query, int limit = 10);
    Task<List<UserDto>> GetOperatorsAsync(bool onlineOnly = false);
}