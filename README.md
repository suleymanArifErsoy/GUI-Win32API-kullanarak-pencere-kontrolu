# Windows Servis Controller

Bu proje, bir **Windows Servisi** ve bir **Konsol Uygulaması**
içermektedir. Servis arka planda çalışarak belirli aralıklarla konsol
uygulamasını çalıştırır. Konsol uygulaması ise o anki aktif pencereyi
kontrol edip üzerinde işlemler gerçekleştirir.

## 📌 Proje Yapısı

1.  **Windows Service (`Service1.cs`)**
    -   Servisin adı **ActiveWindowService** olarak ayarlanmıştır.
    -   Servis başlatıldığında (`OnStart`) her **10 saniyede bir**
        konsol uygulamasını çalıştırır.
    -   Konsol uygulamasını **aktif kullanıcı oturumunda** başlatabilmek
        için `CreateProcessAsUser` API'si kullanılır.
    -   Çalışma ve hata durumları `C:\ActiveWindowServiceLog.txt`
        dosyasına loglanır.
    -   Servis durdurulduğunda (`OnStop`) timer sonlandırılır ve log
        kaydı yapılır.
2.  **Console Application (`Program.cs`)**
    -   `user32.dll` fonksiyonlarını kullanarak aktif pencere üzerinde
        işlem yapar:
        -   Aktif pencere başlığını alır (`GetForegroundWindow`,
            `GetWindowText`).
        -   Pencereyi küçültür (`ShowWindow` - `SW_MINIMIZE`).
        -   Pencereyi büyütür (`ShowWindow` - `SW_MAXIMIZE`).
        -   Pencereyi kapatır (`PostMessage` - `WM_CLOSE`).
    -   İşlemler arası kısa gecikmeler (`Thread.Sleep`) eklenmiştir.

## ⚙️ Çalışma Mantığı

1.  Servis başlatılır.
2.  Servis her 10 saniyede bir konsol uygulamasını çağırır.
3.  Konsol uygulaması:
    -   Aktif pencereyi bulur.
    -   Önce küçültür.
    -   Ardından büyütür.
    -   Son olarak kapatır.
4.  Sonuçlar hem ekranda (Console çıktısı) hem de servis log dosyasında
    görülebilir.

## 🚀 Kullanım

1.  **Servisi yükleme ve başlatma**
    -   Visual Studio ile **Windows Service** projesini build edin.

    -   Servisi yüklemek için:

        ``` powershell
        sc create ActiveWindowService binPath= "C:\...\WindowsServisController.exe"
        sc start ActiveWindowService
        ```

    -   Durdurmak için:

        ``` powershell
        sc stop ActiveWindowService
        ```
2.  **Console uygulamasını manuel çalıştırma**
    -   Konsol uygulamasını tek başına çalıştırabilirsiniz:

        ``` powershell
        WindowsControllerConseleApp.exe
        ```
3.  **Log dosyası**
    -   Servis logları şu dosyada tutulur:

            C:\ActiveWindowServiceLog.txt

## 🔧 Kullanılan Teknolojiler

-   C# (.NET Framework)
-   Windows Service API
-   Win32 API (`user32.dll`, `advapi32.dll`, `Wtsapi32.dll`,
    `kernel32.dll`)

## 📖 Amaç

Bu proje, **Windows servisleri üzerinden GUI oturumunda uygulama
çalıştırma** ve **Win32 API kullanarak pencere kontrolü yapma**
konusunda örnek bir uygulamadır.
