using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPersonalChatRepository
    {
        Task<PersonalChat> CreatePersonalChat (PersonalChat personalChat);
        Task<string> SendMessage (int sender , int reciver ,Message message);
        Task<bool> DeletePesonalChat(int personalChatId);
        Task<bool> MakeBlockChat (int personalChatId,int blockerUserId);
        Task<bool> MakeArchiveChat (int personalChatId);
    }
}
