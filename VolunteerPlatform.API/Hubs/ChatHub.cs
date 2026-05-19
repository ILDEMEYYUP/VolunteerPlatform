using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VolunteerPlatform.Service.Interfaces;

namespace VolunteerPlatform.API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> OnlineUsers = new();
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private string? GetUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                   ?? Context.GetHttpContext()?.Request.Query["userId"].ToString();
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                OnlineUsers.AddOrUpdate(userId, 
                    new HashSet<string> { Context.ConnectionId }, 
                    (key, oldList) => {
                        lock (oldList) { oldList.Add(Context.ConnectionId); }
                        return oldList;
                    });
                await Clients.Others.SendAsync("UserOnlineStatus", userId, true);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                if (OnlineUsers.TryGetValue(userId, out var connections))
                {
                    lock (connections) { connections.Remove(Context.ConnectionId); }
                    if (connections.Count == 0)
                    {
                        OnlineUsers.TryRemove(userId, out _);
                        await Clients.Others.SendAsync("UserOnlineStatus", userId, false);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Mesaj gönderme metodu. Kaydedilen mesajın CorrelationId'sini de fırlatır.
        /// </summary>
        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderIdStr = GetUserId();
            if (string.IsNullOrEmpty(senderIdStr) || string.IsNullOrEmpty(messageContent)) return;

            if (!Guid.TryParse(senderIdStr, out Guid senderId) || !Guid.TryParse(receiverId, out Guid receiverIdGuid))
            {
                throw new HubException("Geçersiz Guid formatı.");
            }

            // 1. Veritabanına kaydet (SCD yapısına göre içeride AddAsync yeni CorrelationId oluşturacak)
            // İpucu: UI'da göstermek için doğrudan MessageService'i bir nesne dönecek şekilde de güncelleyebilirsin.
            // Şimdilik sistemin akışına göre SignalR üzerinden gönderiyoruz:
            
            // Eğer Frontend anlık veriyi almak isterse, MessageService içindeki kayıt sonrası üretilen CorrelationId'yi
            // dönmek için SendMessageAsync metodunu `Task<MessageDto>` yapmayı düşünebilirsin.
            await _messageService.SendMessageAsync(senderId, receiverIdGuid, messageContent);

            // Akışı bozmamak için geçici bir nesne gibi tüm açık istemcilere (Sender ve Receiver) yayın yapıyoruz:
            // Gerçek zamanlı tetiklemede alıcı online ise:
            if (OnlineUsers.TryGetValue(receiverId, out var receiverConnections))
            {
                List<string> activeConnections;
                lock (receiverConnections) { activeConnections = receiverConnections.ToList(); }
                
                // NOT: Gerçek CorrelationId'yi tam senkronize iletmek için dilersen SendMessageAsync metodunu 
                // geriye Guid (CorrelationId) dönecek şekilde refaktör edebilirsin.
                await Clients.Clients(activeConnections).SendAsync("ReceiveMessage", senderIdStr, messageContent, DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Bir mesaj okunduğunda frontend bu hub metodunu tetikleyebilir.
        /// </summary>
        public async Task MessageRead(string senderId, string messageCorrelationId)
        {
            if (Guid.TryParse(messageCorrelationId, out Guid corrId))
            {
                // DB'de SCD tablosuna yeni 'Read' kaydı ekler, eskisini kapatır.
                await _messageService.MarkAsReadAsync(corrId);

                // Gönderen kişiye mesajının okunduğu bilgisini anlık ilet
                if (OnlineUsers.TryGetValue(senderId, out var senderConnections))
                {
                    List<string> activeConnections;
                    lock (senderConnections) { activeConnections = senderConnections.ToList(); }
                    await Clients.Clients(activeConnections).SendAsync("MessageStatusChanged", messageCorrelationId, "Read");
                }
            }
        }
    }
}