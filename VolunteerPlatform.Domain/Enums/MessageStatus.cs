namespace VolunteerPlatform.Domain.Enums;

public enum MessageStatus
{
    // --- Yaşam Döngüsü ---
    Draft = 0,      // Taslak (Henüz gönderilmedi)
    Sent = 1,       // Gönderildi (Sistemden çıktı)
    Delivered = 2,  // Teslim Edildi (Alıcıya ulaştı)
    Read = 3,       // Okundu
    Unread = 4,     // Okunmadı (Okunmamış olarak işaretlendi)

    // --- Hata ve İstisnai Durumlar ---
    Failed = 5,     // Gönderilemedi (Hata oluştu)
    
    // --- Kullanıcı Yönetimi ---
    Archived = 6,   // Arşivlendi (Silinmedi ama görünmüyor)
    Deleted = 7     // Silindi (Soft delete)
}
