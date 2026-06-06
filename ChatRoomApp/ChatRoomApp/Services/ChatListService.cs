using Application.GroupChats.UseCases.Queries;
using Application.PersonalChats.UseCases.Queries;
using Application.Users.UseCases.Queries;
using ChatRoomApp.ViewModels.Chat;
using MediatR;

namespace ChatRoomApp.Services
{
    public class ChatListService : IChatListService
    {
        private readonly IMediator _mediator;

        public ChatListService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<ChatListViewModel>> GetAllChatsByUserIdAsync(int userId)
        {
            var result = new List<ChatListViewModel>();

            // Personal chats
            var personalChats = await _mediator.Send(
                new GetPersonalChatsByUserIdQuery { UserId = userId });

            foreach (var item in personalChats)
            {
                var otherUserId = item.UserOneId == userId ? item.UserTwoId : item.UserOneId;
                var user = await _mediator.Send(new GetUserByIdQuery { UserId = otherUserId });

                result.Add(new ChatListViewModel
                {
                    Id = item.Id,
                    UserId = userId,
                    Name = user?.UserName ?? "Unknown",
                    UserReceiverName = user?.UserName ?? string.Empty,
                    LastMessageText = item.LastMessageText ?? string.Empty,
                    LastMessageTime = (DateTime)item.LastMessageTime,
                    ChatType = "Personal",
                    Description = $"Chat with {user?.UserName ?? "Unknown"}"
                });
            }

            // Group chats
            var groupChats = await _mediator.Send(
                new GetAllGroupByUserIdQuery { UserId = userId });

            foreach (var item in groupChats)
            {
                result.Add(new ChatListViewModel
                {
                    Id = item.Id,
                    UserId = userId,
                    Name = item.Name ?? string.Empty,
                    Description = item.Description ?? string.Empty,
                    LastMessageText = item.LastMessageText ?? string.Empty,
                    LastMessageTime = (DateTime)item.LastMessageTime,
                    ChatType = "Group"
                });
            }

            return result.OrderByDescending(x => x.LastMessageTime).ToList();
        }
    }
}
