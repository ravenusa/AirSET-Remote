# AirSET v2.0 - Automated Infrastructure & Remote System Management

AirSET (Automated Infrastructure & Remote System Management) adalah solusi manajemen laboratorium komputer terpadu berbasis .NET C# WinForms. Sistem ini dirancang untuk mengotomatisasi konfigurasi jaringan, sinkronisasi profil lab, kontrol sistem terpusat, proteksi disk (Unified Write Filter), distribusi dan pengumpulan berkas tugas siswa, live remote monitoring, serta manajemen inventaris perangkat keras dan perangkat lunak secara massal di jaringan lokal (LAN).

---

## Arsitektur Solusi

Sistem AirSET terbagi menjadi beberapa komponen modular:

1. **AirSET.Controller (Host Manager)**
   - Dashboard kontrol pusat untuk instruktur/administrator lab.
   - Mengelola pemindaian cepat (Parallel Fast-Scan) perangkat di subnet LAN.
   - Mengirim konfigurasi IP, Hostname, Workgroup, Password, dan Auto-Logon massal.
   - Mengontrol proteksi disk, power management (Wake-on-LAN, Reboot, Shutdown).
   - Menjalankan distribusi berkas massal dan penarikan tugas (Collect Work).
   - Menampilkan Live Screen Monitoring dan Remote Input (Keyboard & Mouse injection).
   - Menampilkan audit inventaris Hardware dan Software seluruh PC client.

2. **AirSET.Agent (Client Background Service)**
   - Agen background yang berjalan di setiap komputer client/siswa.
   - Menerima dan mengeksekusi instruksi TCP terenkripsi dari Host Controller.
   - Dilengkapi sistem ketahanan (Watchdog Task Scheduler & Service Recovery) agar selalu aktif otomatis.
   - Mendukung eksekusi PowerShell remote, lock screen interaktif, dan pelaporan thumbnail layar real-time.

3. **AirSET.Core (Shared Library)**
   - Pustaka inti bersama yang berisi model data (NetworkPayloads), enkripsi AES-256 (CryptoManager), servis jaringan, dan manajemen disk shield.

4. **AirSET Standalone (Auto IP Setter)**
   - Utilitas portabel mandiri untuk konfigurasi cepat IP, DNS, Workgroup, dan Hostname secara lokal langsung di komputer client tanpa memerlukan Controller.

---

## Fitur Utama

### 1. Manajemen Jaringan dan Identitas Komputer
- Otomatisasi pengaturan IP Address (IPv4), Subnet Mask, Gateway, dan Preferred DNS.
- Penamaan komputer (Hostname) dan pengaturan Workgroup secara terprogram tanpa limitasi NetBIOS.
- Pengaturan User Password Windows dan konfigurasi Sysinternals-grade Auto-Logon (Bypass login screen saat booting namun tetap meminta password jika manual Lock/Sign Out).

### 2. Disk Shield Protection (DeepFry / UWF)
- Integrasi penuh dengan Windows Unified Write Filter (UWF).
- Alokasi Dynamic DISK Overlay cerdas: mengkalkulasi kuota overlay berdasarkan persentase fisik kapasitas drive sistem C: dengan pengaman batas ruang kosong (Safety Guard).
- Pengaturan threshold peringatan (Warning) dan batas kritis (Critical) otomatis.
- Skrip pembersihan exclusion berbahaya dan pencegahan BSOD fast-startup.
- Reference from https://github.com/GPadaka19/DeepFry

### 3. Distribusi dan Pengumpulan Tugas (File Manager)
- Distribusi Berkas Massal: Membagikan materi praktikum, installer, atau dokumen ke seluruh PC client secara serentak dengan throttled streaming.
- Penarikan Tugas (Collect Work): Mengumpulkan berkas tugas siswa dari Desktop/Documents secara serentak ke Host Controller dalam format ZIP terkompresi.
- Mekanisme transmisi aman: Berkas di client hanya dihapus jika transmisi data ke Controller telah diverifikasi sukses 100%.

### 4. Remote Desktop, Live Monitoring, dan Kontrol Daya
- Live Screen Monitoring: Pratinjau tangkapan layar berkala seluruh PC client di dashboard.
- Remote Control & Input Injection: Kontrol mouse dan keyboard jarak jauh secara real-time.
- Kunci Layar Interaktif (Interactive Lock Screen): Mengunci tampilan dan input perangkat client saat sesi ujian atau penjelasan dosen.
- Power Management: Pengiriman Wake-on-LAN, Reboot massal, dan Shutdown massal.

### 5. Audit Inventaris Hardware dan Software
- Pengumpulan spesifikasi perangkat keras (Processor, Total RAM, GPU, Model Motherboard, Disk Storage, MAC Address).
- Pendeteksian seluruh daftar perangkat lunak yang terinstal di client (32-bit & 64-bit Registry scanner).
- Fitur pencarian filter software cepat untuk memeriksa kesiapan aplikasi praktikum.

---

## Keamanan dan Enkripsi

- Komunikasi Jaringan: Seluruh payload instruksi antara Controller dan Agent dienkripsi menggunakan standar AES-256 bit dengan Initial Vector (IV) dinamis.
- Environment Variable Support:
  - `AIRSET_SECRET_KEY`: Kunci sandi enkripsi komunikasi jaringan.
  - `AIRSET_GIST_TOKEN` & `AIRSET_GIST_ID`: Kredensial sinkronisasi profil lab custom via GitHub Gist.

---

## Persyaratan Sistem

- Sistem Operasi: Windows 7 / 8 / 10 / 11 (32-bit dan 64-bit).
- Framework: .NET Framework 4.8 atau lebih baru.
- Hak Akses: Administrator (Run as Administrator) diperlukan untuk modifikasi konfigurasi kartu jaringan, registry, dan driver UWF.

---

## Panduan Build dan Deployment (TODO List)

### Tahap 1: Kompilasi Solusi (Build Application)
1. Buka Terminal / Developer PowerShell pada direktori proyek.
2. Jalankan perintah kompilasi mode Release:
   ```cmd
   dotnet build devIPsett.slnx -c Release
   ```
3. Pastikan proses build menghasilkan `0 Error(s)`. Berkas output eksekusi akan tersedia di:
   - Host Controller: `AirSET.Controller\bin\Release\AirSET.Controller.exe`
   - Client Agent: `AirSET.Agent\bin\Release\AirSET.Agent.exe`
   - Standalone Tool: `bin\Release\AirSET.exe`

### Tahap 2: Deployment di Komputer Client (PC Siswa)
1. Salin berkas `AirSET.Agent.exe` ke direktori lokal di PC client (contoh: `C:\ProgramData\AirSET\` atau `C:\AirSET\`).
2. Jalankan `AirSET.Agent.exe` dengan hak akses Administrator (Klik kanan -> Run as administrator).
3. Agent akan secara otomatis:
   - Mendaftarkan entri Windows Task Scheduler (`AirSETAgent` dan `AirSETWatchdog`).
   - Mengonfigurasi proteksi folder di `C:\ProgramData\AirSET\`.
   - Mengizinkan lalu lintas TCP port 3623 pada Windows Firewall.
   - Menampilkan ikon indikator pada system tray.

### Tahap 3: Deployment di Komputer Host (PC Dosen / Laboran)
1. Salin berkas `AirSET.Controller.exe` ke komputer instruktur/host.
2. Jalankan `AirSET.Controller.exe` dengan hak akses Administrator (Run as administrator).
3. Buat password master aplikasi saat jendela form pertama kali terbuka.
4. Masuk ke Dashboard Manajemen AirSET.

### Tahap 4: Pengoperasian Harian Laboratorium
1. **Pemindaian PC Client:** Pada tab Konfigurasi Jaringan & PC, jalankan Scan Otomatis (Auto) untuk mendeteksi seluruh client yang aktif.
2. **Penerapan Konfigurasi Massal:** Pilih profil lab, tentukan nomor komputer, dan kirim konfigurasi (IP, Hostname, Workgroup, Password, Auto-Logon).
3. **Proteksi Sistem (Freeze):** Buka tab DeepFry (UWF), pilih PC target, dan jalankan Lock Disk (Freeze) untuk mengaktifkan proteksi Dynamic DISK Overlay.
4. **Distribusi & Penarikan Berkas:** Gunakan tab File Manager untuk membagikan materi ujian/tugas dan menarik tugas siswa secara massal ke Host Controller.
5. **Monitoring & Kontrol Daya:** Pantau layar siswa secara real-time via tab Power & Desktop, gunakan fitur Kunci Layar saat penyampaian materi, dan kirim perintah Shutdown/Reboot serentak di akhir jam praktikum.

---

## Struktur Direktori

```text
git-uploadv2/
|-- AirSET.Agent/               # Source code Client Agent
|   |-- AgentListener.cs       # TCP Listener dan eksekutor payload perintah
|   |-- FormLockScreen.cs      # Antarmuka pengunci layar client
|   |-- FormStatus.cs          # UI status koneksi agent
|   |-- TrayAppContext.cs      # Pengelola icon tray dan watchdog service
|   +-- Program.cs             # Entry point client agent
|
|-- AirSET.Controller/          # Source code Host Controller Dashboard
|   |-- FormDashboard.cs       # Antarmuka dashboard utama kontroler
|   |-- FormLogin.cs           # Autentikasi akses kontroler
|   |-- FormChangePassword.cs  # Pengaturan password aplikasi
|   |-- NetworkScanner.cs      # Mesin scanning LAN paralel dan TCP client
|   +-- PasswordAuthManager.cs # Pengelola hashing password kontroler
|
|-- AirSET.Core/                # Pustaka Inti (Class Library)
|   |-- CryptoManager.cs       # Mesin enkripsi AES-256
|   |-- DiskShieldServices.cs  # Layanan integrasi UWF & Deep Freeze
|   |-- NetworkPayloads.cs     # Definisi protokol data transfer
|   |-- NetworkServices.cs     # Operasi sistem jaringan, WMI, dan Netsh
|   +-- LabProfile.cs          # Model data profil laboratorium
|
|-- FormMain.cs                 # Antarmuka utilitas mandiri (Auto IP Setter)
|-- FormAddLab.cs               # Antarmuka tambah/edit profil laboratorium
|-- devIPsett.slnx              # File Solusi Visual Studio
+-- .gitignore                  # Filter pengecualian berkas build dan berkas lokal
```

---

## Kontribusi dan Lisensi

Dikembangkan untuk kebutuhan pengelolaan dan standarisasi infrastruktur laboratorium komputer.
