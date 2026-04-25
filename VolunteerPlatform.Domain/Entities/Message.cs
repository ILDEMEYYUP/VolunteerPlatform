using System;
using VolunteerPlatform.Domain.Enums;

// şuanlık birebir mesajlaşma sistemi 
// fakat ileirde grup chat da getirilebilir
namespace VolunteerPlatform.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;
    
    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
    
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public MessageStatus Status { get; set; } = MessageStatus.Unread;
}
