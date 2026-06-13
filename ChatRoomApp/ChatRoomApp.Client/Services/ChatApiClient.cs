using System.Net.Http.Json;
using ChatRoomApp.Client.Models;

namespace ChatRoomApp.Client.Services;

public class ChatApiClient
{
    private readonly HttpClient _http;

    public ChatApiClient(HttpClient http)
    {
        _http = http;
    }

    // ─── Auth ────────────────────────────────────────────────
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>("/api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    // ─── Users ───────────────────────────────────────────────
    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>($"/api/users/{userId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserDto?> FindUserAsync(string query)
    {
        try
        {
            return await _http.GetFromJsonAsync<UserDto>($"/api/users/search?q={Uri.EscapeDataString(query)}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("/api/users/update", user);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // ─── Personal Chats ──────────────────────────────────────
    public async Task<List<PersonalChatDto>> GetPersonalChatsAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<PersonalChatDto>>($"/api/chats/personal?userId={userId}") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<PersonalChatDto?> GetPersonalChatAsync(int chatId)
    {
        try
        {
            return await _http.GetFromJsonAsync<PersonalChatDto>($"/api/chats/personal/{chatId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<PersonalChatDto?> CreatePersonalChatAsync(int userOneId, int userTwoId)
    {
        var response = await _http.PostAsJsonAsync("/api/chats/personal", new { userOneId, userTwoId });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<PersonalChatDto>();
    }

    // ─── Group Chats ─────────────────────────────────────────
    public async Task<List<GroupDto>> GetGroupChatsAsync(int userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<GroupDto>>($"/api/chats/groups?userId={userId}") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<GroupDto?> GetGroupChatAsync(int groupId)
    {
        try
        {
            return await _http.GetFromJsonAsync<GroupDto>($"/api/chats/groups/{groupId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<GroupDto?> CreateGroupChatAsync(string name, string? description, int ownerId)
    {
        var response = await _http.PostAsJsonAsync("/api/chats/groups", new { name, description, ownerId });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GroupDto>();
    }

    // ─── Messages ────────────────────────────────────────────
    public async Task<List<MessageDto>> GetMessagesAsync(int? personalChatId, int? groupChatId)
    {
        try
        {
            var query = personalChatId.HasValue
                ? $"personalChatId={personalChatId}"
                : $"groupChatId={groupChatId}";
            return await _http.GetFromJsonAsync<List<MessageDto>>($"/api/messages?{query}") ?? new();
        }
        catch
        {
            return new();
        }
    }
}
