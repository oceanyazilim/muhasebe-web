# Panel ile Git Tabanlı Deployment

Bu doküman: **panel.oceanyazilim.com** veya benzeri Coolify/CapRover/Dokploy paneline GitHub'dan otomatik deploy nasıl yapılır.

## 1. Önkoşullar (tek seferlik)

✅ GitHub repo: `https://github.com/oceanyazilim/muhasebe-programi.git` (zaten yapıldı)
✅ Dockerfile: repo kökünde (zaten yapıldı)
✅ .NET 8 runtime: Docker imajı içinde, panelin Linux'unda .NET olması **gerekmez**

## 2. Panel'e Eklenecek Bilgiler

Panel'de yeni proje / environment oluştururken şunları gir:

| Alan | Değer |
|------|-------|
| **Source / Kaynak** | Git Repository |
| **Repository URL** | `https://github.com/oceanyazilim/muhasebe-programi.git` |
| **Branch** | `main` |
| **Build Pack / Build Type** | `Dockerfile` (otomatik tespit edilirse seçili gelir) |
| **Dockerfile Path** | `./Dockerfile` veya `Dockerfile` (kök dizinde) |
| **Build Context** | `.` (proje kökü) |
| **Port** | `8080` |
| **Health Check Path** | `/Identity/Account/Login` |

## 3. Ortam Değişkenleri (Environment Variables)

Panele şu değişkenleri ekle (kritik):

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Data Source=/app/App_Data/muhasebe-web.db
```

**SQLite verisinin kaybolmaması için kalıcı bir volume bağla:**
- Volume Source: `muhasebe-data` (veya host path)
- Mount Path: `/app/App_Data`

## 4. Domain ve SSL

Panel'de:
1. **Domain ekle** → muhasebe.oceanyazilim.com (veya istediğin alt domain)
2. **SSL** → Let's Encrypt otomatik kurulsun
3. **Force HTTPS** → açık

## 5. İlk Deploy

Panel **"Deploy"** veya **"Build & Start"** butonuna bas. İlk build 3-5 dakika sürer:
- `dotnet restore` (paketleri indirir)
- `dotnet publish` (build eder)
- Runtime imajı oluşturur

Loglarda hata varsa görürsün; build başarılıysa container ayağa kalkar.

## 6. Sonraki Deploylar — Sadece Git Push

Yerelden:
```bash
git add .
git commit -m "değişiklik"
git push origin main
```

Panel otomatik olarak:
- Yeni commit'i çekecek
- Yeniden build edecek
- Eski container'ı kapatıp yenisini başlatacak
- Veriler `/app/App_Data` volume'unda saklı kalacak

GitHub webhook genelde otomatik kurulur. Kurulmadıysa panel "Reconnect / Set up webhook" diyor — orayı tıkla.

## 7. İlk Kullanıcı

Site açıldığında `/Identity/Account/Register` sayfasından kayıt ol → giriş yap. **İlk kayıt olan kişi otomatik admin değildir, herkes eşit kullanıcıdır** (her hesap kendi verisini görür).

## 8. Yedekleme

SQLite tek dosyadır: `/app/App_Data/muhasebe-web.db`

Panel üzerinden:
- File Manager / Shell ile dosyayı indir
- Veya container shell aç: `sqlite3 /app/App_Data/muhasebe-web.db ".backup /tmp/backup.db"`

## 9. Hata Ayıklama

Panel container loglarında şunları gör:
- `Application started. Press Ctrl+C to shut down.` → Çalışıyor
- `Now listening on: http://+:8080` → Port doğru
- HealthCheck FAIL → Login endpoint cevap vermiyor

Yaygın sorunlar:
- **App_Data yazılamıyor** → Volume mount eksik veya izinler yanlış
- **Port mismatch** → `EXPOSE 8080` ile panelde girilen port aynı olmalı
- **Memory limit** → Blazor Server WebSocket için min 256 MB tavsiye edilir

## 10. Güvenlik Notları

- ⚠️ İlk kayıt olan hesabı admin olarak işaretlemek istersen Program.cs'ye seed kodu eklenebilir; söyleyin yazayım
- Şifre kuralları: en az 6 karakter + 1 rakam (Program.cs'den ayarla)
- HTTPS zorlanır (panelde Force HTTPS açık olmalı)
- Her kullanıcı sadece kendi `UserId`'sine ait veriyi görür (kod seviyesinde filtreleniyor)
