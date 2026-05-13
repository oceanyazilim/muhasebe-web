using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MuhasebeApp.Web.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(200)] public string? AdSoyad { get; set; }
    [MaxLength(200)] public string? SirketAdi { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
}

public enum CariTipi { Musteri = 0, Tedarikci = 1, HerIkisi = 2 }
public enum HareketTipi { Borc = 0, Alacak = 1 }
public enum OdemeYonu { Alacak = 0, Borc = 1 }
public enum OdemeDurumu { Beklemede = 0, Odendi = 1, KismiOdendi = 2, Gecikti = 3, Iptal = 4 }
public enum FaturaYonu { Gonderilen = 0, Gelen = 1 }
public enum FaturaDurumu { Beklemede = 0, Odendi = 1, KismiOdendi = 2, Iptal = 3 }

public class Cari
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Kod { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string Unvan { get; set; } = string.Empty;
    public CariTipi Tip { get; set; } = CariTipi.Musteri;
    [MaxLength(50)] public string? VergiNo { get; set; }
    [MaxLength(100)] public string? VergiDairesi { get; set; }
    [MaxLength(20)] public string? TcKimlikNo { get; set; }
    [MaxLength(50)] public string? Telefon { get; set; }
    [MaxLength(150)] public string? Email { get; set; }
    [MaxLength(500)] public string? Adres { get; set; }
    [MaxLength(100)] public string? Sehir { get; set; }
    public DateTime AcilisTarihi { get; set; } = DateTime.Today;
    public decimal AcilisBakiyesi { get; set; } = 0;
    public string? Notlar { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    public string TipMetni => Tip switch
    {
        CariTipi.Musteri => "Müşteri",
        CariTipi.Tedarikci => "Tedarikçi",
        CariTipi.HerIkisi => "Müşteri/Tedarikçi",
        _ => "-"
    };
}

public class Hareket
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int CariId { get; set; }
    public Cari? Cari { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Today;
    public HareketTipi Tip { get; set; }
    public decimal Tutar { get; set; }
    [MaxLength(10)] public string ParaBirimi { get; set; } = "TRY";
    [MaxLength(1000)] public string? Aciklama { get; set; }
    [MaxLength(100)] public string? BelgeNo { get; set; }
    public DateTime? VadeTarihi { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    public string TipMetni => Tip == HareketTipi.Borc ? "Borç" : "Alacak";
}

public class OdemeNotu
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int CariId { get; set; }
    public Cari? Cari { get; set; }
    public OdemeYonu Yon { get; set; } = OdemeYonu.Alacak;
    [Required, MaxLength(250)] public string Baslik { get; set; } = string.Empty;
    [MaxLength(2000)] public string? Aciklama { get; set; }
    public decimal Tutar { get; set; }
    public decimal OdenenTutar { get; set; } = 0;
    [MaxLength(10)] public string ParaBirimi { get; set; } = "TRY";
    public DateTime VadeTarihi { get; set; } = DateTime.Today.AddDays(30);
    public OdemeDurumu Durum { get; set; } = OdemeDurumu.Beklemede;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? OdemeTarihi { get; set; }

    public decimal KalanTutar => Tutar - OdenenTutar;
    public int GunFarki => (VadeTarihi.Date - DateTime.Today).Days;
    public bool VadesiGecmis => GunFarki < 0 && (Durum == OdemeDurumu.Beklemede || Durum == OdemeDurumu.KismiOdendi);
    public string YonMetni => Yon == OdemeYonu.Alacak ? "Alacak" : "Borç";
    public string DurumMetni => Durum switch
    {
        OdemeDurumu.Beklemede => VadesiGecmis ? "Gecikti" : "Beklemede",
        OdemeDurumu.Odendi => "Tamamlandı",
        OdemeDurumu.KismiOdendi => "Kısmi",
        OdemeDurumu.Iptal => "İptal",
        _ => "-"
    };
}

public class Fatura
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int CariId { get; set; }
    public Cari? Cari { get; set; }
    public FaturaYonu Yon { get; set; } = FaturaYonu.Gonderilen;
    [Required, MaxLength(50)] public string FaturaNo { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.Today;
    public DateTime? VadeTarihi { get; set; }
    [Required, MaxLength(2000)] public string IsAciklamasi { get; set; } = string.Empty;
    public decimal Tutar { get; set; }
    public decimal KdvOrani { get; set; } = 20;
    public decimal KdvTutari { get; set; }
    public decimal ToplamTutar { get; set; }
    [MaxLength(10)] public string ParaBirimi { get; set; } = "TRY";
    public FaturaDurumu Durum { get; set; } = FaturaDurumu.Beklemede;
    public decimal OdenenTutar { get; set; } = 0;
    [MaxLength(2000)] public string? Notlar { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    public decimal KalanTutar => ToplamTutar - OdenenTutar;
    public string YonMetni => Yon == FaturaYonu.Gonderilen ? "Gönderilen" : "Gelen";
    public string DurumMetni => Durum switch
    {
        FaturaDurumu.Beklemede => "Beklemede",
        FaturaDurumu.Odendi => "Ödendi",
        FaturaDurumu.KismiOdendi => "Kısmi Ödendi",
        FaturaDurumu.Iptal => "İptal",
        _ => "-"
    };
}

public class Dekont
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int? CariId { get; set; }
    public int? OdemeNotuId { get; set; }
    public int? FaturaId { get; set; }
    public int? HareketId { get; set; }
    [Required, MaxLength(500)] public string DosyaAdi { get; set; } = string.Empty;
    [Required, MaxLength(500)] public string DepoYolu { get; set; } = string.Empty;
    public long Boyut { get; set; }
    [MaxLength(150)] public string? MimeTip { get; set; }
    [MaxLength(1000)] public string? Aciklama { get; set; }
    public DateTime EklemeTarihi { get; set; } = DateTime.UtcNow;
}
