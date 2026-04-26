using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.Service.Interfaces
{
    public interface IMessageService : IGenericService<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id);
        Task SendMessageAsync(Guid senderId, Guid receiverId, string content);
        Task MarkAsReadAsync(Guid messageId);
        Task MarkAsUnreadAsync(Guid messageId);
        Task SoftDeleteMessageAsync(Guid messageId);
        Task ArchiveMessageAsync(Guid messageId);
    }
}
