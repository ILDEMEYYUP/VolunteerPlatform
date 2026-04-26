using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Data;
using VolunteerPlatform.Domain.Entities;
using VolunteerPlatform.Domain.Enums;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.Service.Concrete
{
    public class MessageService : GenericService<Message>, IMessageService
    {
        public MessageService(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) || 
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .Where(m => m.Status != MessageStatus.Deleted) // Ignore soft deleted
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task SendMessageAsync(Guid senderId, Guid receiverId, string content)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                Status = MessageStatus.Sent
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null && message.Status != MessageStatus.Deleted)
            {
                message.Status = MessageStatus.Read;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsUnreadAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null && message.Status != MessageStatus.Deleted)
            {
                message.Status = MessageStatus.Unread;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteMessageAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.Status = MessageStatus.Deleted;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ArchiveMessageAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null && message.Status != MessageStatus.Deleted)
            {
                message.Status = MessageStatus.Archived;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
