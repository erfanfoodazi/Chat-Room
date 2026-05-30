using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> CreateMessageAsync(Message message);
        Task<Message> EditMessageAsync(Message message);
        Task<bool> DeleteMessageAsync(int meesageId);
        Task<bool> MakeMessageDeliveredAsync(int messageId);
        Task<bool> MakeMessageSeenAsync(int messageId);
        Task<List<Message>> GetMessagesByPersonalChatIdAsync(int personalChatId, int skip = 0, int take = 50);
        Task<List<Message>> GetMessagesByGroupChatIdAsync(int groupChatId, int skip = 0, int take = 50);
        Task<Message> GetMessageByIdAsync(int messageId);
    }
}
