using Application.GroupChats.UseCases.Queries;
using Application.PersonalChats.UseCases.Queries;
using Application.Users.UseCases.Queries;
using MediatR;

namespace ChatRoomApp.ViewModels.Chat
{
    public class ChatListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserReceiverName { get; set; } = string.Empty;
        public string LastMessageText { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public string ChatType { get; set; } = string.Empty; 

        public ChatListViewModel()
        {
        }

        public static async Task<List<ChatListViewModel>> GetAllChatsByUserIdAsync(
            int userId,
            IMediator mediator)
        {
            var result = new List<ChatListViewModel>();

            // Get personal chats
            var personalChats = await mediator.Send(
                new GetPersonalChatsByUserIdQuery { UserId = userId });

            foreach (var item in personalChats)
            {
                var chatViewModel = new ChatListViewModel
                {
                    Id = item.Id,
                    UserId = userId,
                    LastMessageText = item.LastMessageText ?? string.Empty,
                    LastMessageTime = (DateTime)item.LastMessageTime,
                    ChatType = "Personal"
                };

                // Determine the other user's name
                var otherUserId = item.UserOneId == userId ? item.UserTwoId : item.UserOneId;
                var userQuery = new GetUserByIdQuery { UserId = otherUserId }; // Assuming you have this query
                var user = await mediator.Send(userQuery);

                chatViewModel.Name = user?.UserName ?? "Unknown User";
                chatViewModel.UserReceiverName = user?.UserName ?? string.Empty;
                chatViewModel.Description = $"Chat with {chatViewModel.Name}";

                result.Add(chatViewModel);
            }

            // Get group chats
            var groupChats = await mediator.Send(
                new GetAllGroupByUserIdQuery { UserId = userId });

            foreach (var item in groupChats)
            {
                var chatViewModel = new ChatListViewModel
                {
                    Id = item.Id,
                    UserId = userId,
                    Name = item.Name ?? string.Empty,
                    Description = item.Description ?? string.Empty,
                    LastMessageText = item.LastMessageText ?? string.Empty,
                    LastMessageTime = (DateTime)item.LastMessageTime,
                    ChatType = "Group",
                    UserReceiverName = string.Empty // Not applicable for groups
                };

                result.Add(chatViewModel);
            }

            // Sort by last message time (most recent first)
            return result.OrderByDescending(x => x.LastMessageTime).ToList();
        }
    }
}

// Additional query if needed
namespace Application.Users.UseCases.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public int UserId { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}