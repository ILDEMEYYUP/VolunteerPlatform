using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerPlatform.Domain.Entities ; 
        // BU KISIM İLERSİ İÇİN TASARLANDI 
        // şikaet etme özelliği şart 
        // fakat şuan eklemeye gerek yok 
// public class Report
// {
//     public int Id { get; set; }
//     public string Reason { get; set; } = null!; // Şikayet sebebi
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     public bool IsResolved { get; set; } = false; // Admin baktı mı?

//     // Kim şikayet etti
//     public int ReporterId { get; set; }
//     public User Reporter { get; set; } = null!;

//     // Hangi mesaj şikayet edildi
//     public int ReportedMessageId { get; set; }
//     public Message ReportedMessage { get; set; } = null!;
// }