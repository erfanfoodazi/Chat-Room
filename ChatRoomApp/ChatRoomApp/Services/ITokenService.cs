using Domain.Entities;

namespace ChatRoomApp.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user);
    DateTime GetExpiry();
}
