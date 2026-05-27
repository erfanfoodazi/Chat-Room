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
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MessageRepository> _logger;
        public MessageRepository(AppDbContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Message> CreateMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();    
            return message;
        }

        public async Task<bool> DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if(message == null)
            {
                _logger.LogError("message not found ,id : {messageId}",messageId);
                return false;
            }
            message.Delete();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Message> EditMessage(Message message)
        {
            var existMessage = await _context.Messages.FindAsync(message.Id);
            if (existMessage == null)
            {
                _logger.LogError("Message not found with id : {messageId}",message.Id);
                throw new NotFoundException("not found");
            }
            existMessage.Edit(message.Text);
            await _context.SaveChangesAsync();
            return existMessage;
        }

        public async Task<Message> GetMessageById(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if(message == null)
            {
                _logger.LogError("message with id {id} not found", messageId);
                throw new NotFoundException($"not found{messageId}");
            }
            return message;
        }

        public async Task<List<Message>> GetMessagesByGroupChatId(int groupChatId, int skip = 0, int take = 50)
        {
            var messages = await _context.Messages
                .Where(m => m.GroupChatId == groupChatId)
                .OrderBy(m => m.SentTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return messages;
        }

        public async Task<List<Message>> GetMessagesByPersonalChatId(int personalChatId,int skip = 0, int take = 50)
        {
            var messages = await _context.Messages
                .Where(m => m.PersonalChatId == personalChatId)
                .OrderBy(m => m.SentTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return messages;
        }

        public async Task<bool> MakeMessageDelivered(int messageId)
        {
            var message = await _context.Messages
                .FindAsync(messageId);
            if( message == null )
            {
                _logger.LogError("message not found ,id : {messageId}", messageId);
                return false;
            }
            message.MarkAsDelivered();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MakeMessageSeen(int messageId)
        {
            var message = await _context.Messages
               .FindAsync(messageId);
            if (message == null)
            {
                _logger.LogError("message not found ,id : {messageId}", messageId);
                return false;
            }
            message.MarkAsSeen();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
