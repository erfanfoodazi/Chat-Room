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
        Task<Message> CreateMessage(Message message);
        Task<Message> EditMessage(Message message);
        Task<bool> DeleteMessage(int meesageId);
        Task<bool> MakeMessageDelivered(int messageId);
        Task<bool> MakeMessageSeen(int messageId);
        Task<List<Message>> GetMessagesByPersonalChatId(int personalChatId, int skip = 0, int take = 50);
        Task<List<Message>> GetMessagesByGroupChatId(int groupChatId, int skip = 0, int take = 50);
        Task<Message> GetMessageById(int messageId);
    }
}
