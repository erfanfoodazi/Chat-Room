using ChatRoomApp.ViewModels.Chat;

namespace ChatRoomApp.Services
{
    public interface IChatListService
    {
        Task<List<ChatListViewModel>> GetAllChatsByUserIdAsync(int userId);
    }
}
