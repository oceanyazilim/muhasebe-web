# Muhasebe Pro Web

Çok kullanıcılı, web tabanlı muhasebe uygulaması. **ASP.NET Core 8 + Blazor Server + Identity + SQLite**.

🌐 **Canlı**: https://muhasebe.oceanyazilim.com

## Özellikler

- **Çok kullanıcılı:** Her hesap kendi e-posta + şifre ile kayıt olur, sadece kendi verisini görür
- **Anasayfa:** Alacak/Borç/Net Pozisyon kartları, yaklaşan vade listeleri
- **Profiller:** Alıcı/borçlu profilleri, canlı arama
- **Faturalar:** Gönderilen ve gelen fatura kaydı, KDV otomatik hesap, opsiyonel ödeme notu
- **Alacaklar / Borçlar:** Vade takibi, tek tıkla tahsilat/ödeme
- **Hareketler:** Tüm para hareketleri
- **Responsive:** Bootstrap 5.3 + Bootstrap Icons, mobil ve masaüstünden çalışır

## Tech Stack

- ASP.NET Core 8 / Blazor Server (Interactive)
- ASP.NET Core Identity (e-posta + şifre auth)
- Entity Framework Core 8 + SQLite
- Bootstrap 5.3 + Bootstrap Icons (CDN)
- Docker (multi-stage build)

## Yerel Çalıştırma

```bash
git clone https://github.com/oceanyazilim/muhasebe-web.git
cd muhasebe-web
dotnet run
```

`http://localhost:5000` → `/Identity/Account/Register` ile kayıt ol → kullanmaya başla.

## Docker ile Çalıştırma

```bash
docker build -t muhasebe-web .
docker run -d -p 8080:8080 -v muhasebe-data:/app/App_Data --name muhasebe muhasebe-web
```

`http://localhost:8080` adresinden eriş.

## Dokploy / Coolify Deployment

Panele şunları gir:

- **Repository URL:** `https://github.com/oceanyazilim/muhasebe-web.git`
- **Branch:** `main`
- **Build Type:** Dockerfile
- **Dockerfile Path:** `Dockerfile`
- **Container Port:** `8080`
- **Volume:**
  - Source: `muhasebe-data`
  - Mount Path: `/app/App_Data`
- **Environment Variables:**
  ```
  ASPNETCORE_ENVIRONMENT=Production
  ConnectionStrings__DefaultConnection=Data Source=/app/App_Data/muhasebe-web.db
  ```

## Veri Yedekleme

SQLite tek dosya: `App_Data/muhasebe-web.db`

```bash
docker cp muhasebe:/app/App_Data/muhasebe-web.db ./backup-$(date +%Y%m%d).db
```

## Lisans

© Ocean Yazılım — Tüm hakları saklıdır.
