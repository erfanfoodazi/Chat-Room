using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GroupChatRepository> _logger;

        public GroupChatRepository(AppDbContext context, ILogger<GroupChatRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AddUserToGroupId(int groupId, User user)
        {
             var groupChat = await _context.GroupChats
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (groupChat == null)
            {
                _logger.LogError("Group chat not found  {groupId}",groupId);
                return false;
            }

            var existUserInGroup = groupChat.Members.FirstOrDefault(m => m.UserId == user.Id);
            if (existUserInGroup != null)
            {
                _logger.LogWarning("{userName} already is join in {GroupName} group",user.UserName,groupChat.Name);
                return false;
            }
            var member = groupChat.CreateMember(user, groupId);
            groupChat.Members.Add(member);
            await _context.SaveChangesAsync();
            return true;
                
        }

        public async Task<GroupChat> CreateGroupChat(GroupChat groupChat, User owner)
        {
            groupChat.OwnerId = owner.Id;
            await _context.GroupChats.AddAsync(groupChat);
            await _context.SaveChangesAsync();

            var member = groupChat.CreateOwnerUser(owner);
            groupChat.Members.Add(member);

            await _context.SaveChangesAsync();
            return groupChat;
        }

        public async Task<bool> DeleteGroupChat(int groupId)
        {
            var group = await _context.GroupChats
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
            if(group == null)
            {
                _logger.LogError("Group chat not found  {groupId}", groupId);
                return false;
            }
            _context.GroupChats.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<GroupChat>> GetAllGroupChatByUserId(int userId)
        {
            var groupChatIds = await _context.GroupMembers
                .Where(m => m.UserId == userId)
                .Select(m => m.GroupChatId)
                .ToListAsync();

            if (!groupChatIds.Any())
            {
                _logger.LogWarning("User {userId} has no group chats", userId);
                return new List<GroupChat>();
            }

            return await _context.GroupChats
                .Where(g => groupChatIds.Contains(g.Id))
                .ToListAsync();
        }

        public async Task<GroupChat> GetGroupChatByGroupId(int groupId)
        {
            var group = await _context.GroupChats.FindAsync(groupId);
            if (group == null)
            {
                _logger.LogWarning("Group Chat not found {GroupId}",groupId);
                return null;
            }
            return group;
        }

        public async Task<string> SendMessage(Message message)
        {
            var group = await _context.GroupChats
                .FirstOrDefaultAsync(g => g.Id == message.GroupChatId);
            if (group == null)
            {
                _logger.LogError("Group not found {groupId}", message.GroupChatId);
                return string.Empty;
            }
            group.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message.Text;
        }

        public async Task<bool> UpdateGroupChat(GroupChat groupChat)
        {
            var group = await _context.GroupChats
                .FirstOrDefaultAsync(g => g.Id == groupChat.Id);
            if (group == null)
            {
                _logger.LogError("This group not found {groupId}",groupChat.Id);
                return false;
            }

            group.Name = groupChat.Name;
            group.Description = groupChat.Description;
            group.ProfilePictureUrl = groupChat.ProfilePictureUrl;
            group.IsPublic = groupChat.IsPublic;
            group.OwnerId = groupChat.OwnerId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
