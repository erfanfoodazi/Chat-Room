namespace ChatRoomApp.Services;

public class UserStatusBroadcastService
{
    public event Action<int, bool>? OnUserStatusChanged;

    public void NotifyStatusChanged(int userId, bool isOnline)
    {
        OnUserStatusChanged?.Invoke(userId, isOnline);
    }
}
