# Muhasebe Pro Web

Çok kullanıcılı, web tabanlı muhasebe uygulaması. **Blazor Server (.NET 8) + ASP.NET Core Identity + SQLite** ile yazıldı.

Mevcut WinUI 3 masaüstü uygulamasıyla aynı modeli ele alır ama bağımsız bir projedir.

## Özellikler

- **Çok kullanıcılı:** Her kullanıcı kendi e-posta + şifre ile kayıt olur, kendi verilerini görür
- **Anasayfa:** Alacak/Borç/Net Pozisyon kartları, yaklaşan vade listeleri
- **Profiller:** Alıcı/borçlu profilleri, canlı arama
- **Faturalar:** Gönderilen ve gelen fatura kaydı, KDV otomatik hesap, opsiyonel ödeme notu
- **Alacaklar / Borçlar:** Vade takibi, tek tıkla tahsilat/ödeme
- **Hareketler:** Tüm para hareketleri
- **Responsive:** Bootstrap 5.3, mobil ve masaüstünden çalışır

## Yerel çalıştırma

```powershell
cd C:\dev\maxvaro\MuhasebeApp.Web
dotnet restore
dotnet run
```

`https://localhost:5001` adresinden açılır. İlk açılışta SQLite veritabanı oluşur (`App_Data/muhasebe-web.db`).

## Plesk / Windows Hosting Deployment

### 1) Önkoşullar
Sunucunuzda kurulu olmalı:
- **.NET 8 Hosting Bundle** (https://dotnet.microsoft.com/download/dotnet/8.0 → "Hosting Bundle")
- IIS + ASP.NET Core Module V2

Plesk panelinde hosting planının **.NET 8** desteklediğini kontrol edin. Çoğu Türk hostingde "ASP.NET" altında sürüm seçilebilir.

### 2) Publish

Yerel makinede tek komutla yayına hazır çıktı oluştur:
```powershell
cd C:\dev\maxvaro\MuhasebeApp.Web
dotnet publish -c Release -o ./publish --self-contained false
```

`publish/` klasöründeki **tüm dosyaları** Plesk'in `httpdocs` veya `wwwroot` klasörüne FTP/SFTP/Plesk File Manager ile yükleyin.

Önemli dosyalar:
- `MuhasebeApp.Web.dll` — uygulama
- `web.config` — IIS yapılandırması (zaten hazır)
- `appsettings.json` — config
- `App_Data/` — SQLite veritabanı buraya oluşur

### 3) İzinler

`App_Data/` klasörü için **yazma izni** ver:
- Plesk → Dosya Yöneticisi → `App_Data` klasörüne sağ tık → İzinler → IIS_IUSRS'a "Write" yetkisi

### 4) İlk çalıştırma

Site adresine git → otomatik `/Identity/Account/Register` veya `/Identity/Account/Login` sayfasına yönlendirir → kayıt ol → giriş yap → kullanmaya başla.

## GitHub'dan Otomatik Deploy

Plesk'in **Git** modülünü kullanarak push'ladığında otomatik deploy edebilirsin:

1. Plesk → **Git** → Repo URL: `https://github.com/oceanyazilim/muhasebe-programi.git`
2. Branch: `main`
3. Deployment Target Directory: site kök dizini
4. **Additional deployment actions** (Plesk shell scripti):
   ```bash
   cd MuhasebeApp.Web
   dotnet publish -c Release -o ../publish_temp --self-contained false
   cp -r ../publish_temp/* ../
   rm -rf ../publish_temp
   ```
   (Plesk Linux ise; Windows Plesk için PowerShell eşdeğeri)

Her `git push origin main` sonrası Plesk otomatik çekip publish eder.

## Veritabanı Yedekleme

Veritabanı `App_Data/muhasebe-web.db` tek bir SQLite dosyasıdır.
- Yedek: Plesk Dosya Yöneticisi'nden indir
- Geri yükle: Aynı dosyayı üzerine yaz, app pool'u restart et (Plesk → "Restart")

## SSL / HTTPS

Plesk'te **Let's Encrypt** uzantısını yükle, sertifikayı oluştur, force HTTPS aç. Uygulama zaten `UseHttpsRedirection()` ile geliyor.

## Geliştirme Notları

- `Models/Entities.cs` — tüm veri sınıfları
- `Data/AppDbContext.cs` — EF Core context
- `Components/Pages/` — Blazor sayfaları
- `Program.cs` — DI ve middleware
- Şifre kuralları: en az 6 karakter, en az 1 rakam (Program.cs'den ayarla)
- Her kullanıcı kendi `UserId`'sine göre veri görür — tüm sorgularda `.Where(x => x.UserId == _userId)` filtresi var
