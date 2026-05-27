using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PersonalChatRepository : IPersonalChatRepository
    {
        private readonly ILogger<PersonalChatRepository> _logger;
        private readonly AppDbContext _context;
        public PersonalChatRepository(ILogger<PersonalChatRepository> logger,AppDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PersonalChat> CreatePersonalChat(PersonalChat personalChat)
        {
            await _context.PersonalChats.AddAsync(personalChat);
            await _context.SaveChangesAsync();
            return personalChat;
        }

        public async Task<bool> DeletePesonalChat(int personalChatId)
        {
            var personalChat = await GetPersonalChatById(personalChatId);
            if (personalChat == null)
            {
                _logger.LogError("Personal chat not found!");
                return false;
            }
            _context.PersonalChats.Remove(personalChat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PersonalChat> GetPersonalChatById(int personalChatId)
        {
            var result = await _context.PersonalChats
                .FindAsync(personalChatId);
            if (result == null)
            {
                _logger.LogError("Personal chat {Id} not found", personalChatId);
                throw new NotFoundException($"PersonalChat with ID {personalChatId} not found");
            }
            return result;
        }
        public async Task<List<PersonalChat>> GetPersonalChatsByUserId(int userId)
        {
            var chats = await _context.PersonalChats
                .Where(p => p.UserOneId == userId || p.UserTwoId == userId)
                .ToListAsync();
            if(!chats.Any())
            {
                _logger.LogWarning("There is no chat for this user");
            }
            return chats;
        }

        public async Task<bool> MakeArchiveChat(int personalChatId)
        {
            var chat = await GetPersonalChatById(personalChatId);
            if(chat == null)
            {
                _logger.LogError("Personal chat not found!");
                return false;
            }
            chat.IsArchived = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MakeBlockChat(int personalChatId, int blockerUserId)
        {
            var personalChat = await GetPersonalChatById(personalChatId);
            if (personalChat == null)
            {
                _logger.LogError("Personal chat not found!");
                return false;
            }
            personalChat.BlockedByUserId = blockerUserId;
            personalChat.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<string> SendMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message.Text;

        }
    }
}
