namespace Domain.Models;

    // Status kodları üçün Enum (0 = Qeyri-aktiv/Silinib, 1 = Aktiv)
    public enum EntityStatus
    {
        Deleted = 0,
        Active = 1
    }

    public abstract class BaseEntity
    {

        // Audit Zaman Göstəriciləri
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }

        // Status kodu (Default olaraq Aktiv təyin edilir)
        public EntityStatus Status { get; set; } = EntityStatus.Active;

        // İstifadəçi Məlumatları (UserId-lərin tipi proyektinizə uyğun olaraq string və ya int ola bilər)
        public int InsertByUserId { get; set; }
        public int? UpdateByUserId { get; set; }
       
    }
