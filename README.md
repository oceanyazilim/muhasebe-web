# Muhasebe Pro Web

Çok kullanıcılı, web tabanlı muhasebe uygulaması. **ASP.NET Core 8 + Blazor Server + Identity + PostgreSQL**.

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
- Entity Framework Core 8 + **PostgreSQL (Npgsql)**
- Bootstrap 5.3 + Bootstrap Icons (CDN)
- Docker (multi-stage build), Traefik reverse proxy uyumlu
- Healthcheck endpoint: `/health`

## Yerel Çalıştırma

```bash
git clone https://github.com/grxtor/muhasebe-web.git
cd muhasebe-web

# PostgreSQL bağlantısını user-secrets ile ver (commit'lenmez)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "postgresql://USER:PASS@HOST:5432/DB"

dotnet run
```

veya `.env` ile:

```bash
cp .env.example .env
# .env içine PostgreSQL DSN'inizi yazın
export $(grep -v '^#' .env | xargs)
dotnet run
```

`http://localhost:5000` → `/Identity/Account/Register` ile kayıt ol → kullanmaya başla.

## Docker ile Çalıştırma

```bash
docker build -t muhasebe-web .
docker run -d -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="postgresql://USER:PASS@HOST:5432/DB" \
  -v muhasebe-data:/app/App_Data \
  --name muhasebe muhasebe-web
```

`http://localhost:8080` adresinden eriş.

## Dokploy Deployment

1. **Application** olarak repo'yu bağla:
   - Repository: `https://github.com/grxtor/muhasebe-web.git`
   - Branch: `main`
   - Build Type: `Dockerfile`

2. **Environment Variables** ekle:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=postgresql://postgres:PASSWORD@HOST:5432/postgres
   ```

3. **Domains** sekmesinden domain ekle:
   - Host: `muhasebe.oceanyazilim.com`
   - Container Port: `8080`
   - HTTPS: aktif (Let's Encrypt)

4. **Volume** (opsiyonel — dekont dosyaları için):
   - Source: `muhasebe-data`
   - Mount: `/app/App_Data`

5. **Deploy** → İlk açılışta tablolar otomatik oluşturulur (`EnsureCreated`).

## Veri Yedekleme

PostgreSQL dump:

```bash
pg_dump "postgresql://USER:PASS@HOST:5432/DB" > backup-$(date +%Y%m%d).sql
```

## Lisans

© Ocean Yazılım — Tüm hakları saklıdır.
