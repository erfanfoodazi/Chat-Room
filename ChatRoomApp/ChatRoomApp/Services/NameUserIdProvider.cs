using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApp.Services;

public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
