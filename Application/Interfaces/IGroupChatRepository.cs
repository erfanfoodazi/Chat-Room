using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGroupChatRepository
    {
        Task<List<GroupChat>> GetAllGroupChatByUserId(int userId);
        Task<GroupChat> GetGroupChatByGroupId(int groupId);
        Task<GroupChat> CreateGroupChat(GroupChat groupChat, User owner);
        Task<bool> UpdateGroupChat(GroupChat groupChat);
        Task<bool> DeleteGroupChat(int groupId);
        Task<bool> AddUserToGroupId(int groupId, User user);
        Task<bool> RemoveUserFromGroup(int groupId, int userId);
        Task<GroupRole?> GetGroupMemberRole(int groupId, int userId);
        Task<string> SendMessage(Message message);
    }
}
