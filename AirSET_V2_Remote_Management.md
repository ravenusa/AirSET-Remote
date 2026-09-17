# 🚀 AirSET v2.0 - Remote Management Extension (Client-Server Architecture)

Pengembangan lanjutan dari AirSET yang memungkinkan pengaturan infrastruktur lab dilakukan secara **terpusat (Remote Management)** dari satu komputer pusat (Host), tanpa perlu mendatangi dan menyetel PC Client satu per satu.

## 🏗️ Arsitektur Sistem Baru

Sistem AirSET kini dibagi menjadi dua entitas utama:

### 1. 🖥️ AirSET Controller (Host/Server)
Aplikasi antarmuka utama yang digunakan oleh Admin/Asisten Lab di komputer pusat.
* **Network Scanner:** Menemukan (Auto-Discover) semua komputer yang terhubung dalam satu jaringan Lab secara otomatis.
* **Live Dashboard:** Menampilkan daftar PC (Hostname, IP Address saat ini, Status Online/Offline, MAC Address).
* **Batch Configuration:** Memilih beberapa atau semua PC sekaligus untuk dikonfigurasi secara massal (Auto-Increment IP, Change Hostname, Change Workgroup).
* **Remote Execution:** Mengirim perintah eksekusi jaringan, mematikan (Shutdown), merestart (Restart), atau menyalakan PC via Wake-on-LAN (WOL).

### 2. 🛡️ AirSET Agent (Client/Receiver)
Aplikasi ringan (System Tray / Windows Service) yang berjalan di latar belakang (Background) pada komputer client.
* **Always Listening:** Berjalan dengan hak akses Administrator dan bersiap menerima koneksi (listen) di port spesifik (misal: Port 3623).
* **Silent Execution:** Mengeksekusi command jaringan (Netsh, PowerShell, WMI) secara *silent/hidden* berdasarkan instruksi dari Host.
* **Auto-Start:** Otomatis berjalan saat Windows booting untuk memastikan PC selalu siap diremote oleh Host.
* **Security Token:** Memvalidasi perintah menggunakan *Pre-Shared Key / AES Encryption* agar terhindar dari serangan siber (hijacking) oleh pihak tidak berwenang.

---

## 🔑 Fitur Tambahan (Remote Features)

* 📡 **UDP Auto-Discovery:** Client melakukan *broadcast* kehadirannya ke jaringan lokal, sehingga Host bisa langsung membuat daftar PC yang aktif.
* ⚡ **One-Click Mass Setup:** Konfigurasi 40+ komputer Lab dalam hitungan detik. (Contoh: Host menyuruh PC 1 hingga 40 untuk urut IP 192.168.1.1 sampai 192.168.1.40).
* 🔄 **Live Status Feedback:** Client mengirimkan log balik (Response) ke Host apakah perintah `netsh` atau perubahan `hostname` berhasil dilakukan atau gagal.
* 🔒 **Encrypted Payload:** Komunikasi perintah menggunakan format JSON yang dienkripsi agar aman di dalam LAN.

---

## 🛠️ Persyaratan Sistem Tambahan

* **Jaringan:** Host dan Client harus berada dalam segmen jaringan (VLAN/Subnet) yang sama untuk fitur UDP Auto-Discovery.
* **Firewall Rules:** Windows Defender Firewall pada PC Client harus diizinkan (Allowed Exception) untuk Port Inbound AirSET Agent (misal: TCP/UDP Port 3623).
* **Hak Akses Agent:** AirSET Agent **wajib** dipasang/dijalankan menggunakan *Administrator Privileges* di PC Client.

---

## 💻 Pembaruan Struktur Project (Update)

```text
git-upload/
├── AirSET.Controller/               # [NEW] Aplikasi Pusat (Host)
│   ├── FormDashboard.cs             # Antarmuka Monitoring & Remote Control
│   ├── NetworkScanner.cs            # Modul UDP Broadcast & TCP Client Sender
│   └── ControllerConfig.json        # Konfigurasi Host
│
├── AirSET.Agent/                    # [NEW] Aplikasi Penerima (Client)
│   ├── AgentService.cs              # TCP Listener (Menunggu Perintah)
│   ├── CommandExecutor.cs           # Integrasi ke NetworkServices yang lama
│   └── SecurityAuth.cs              # Validasi Token Enkripsi
│
├── AirSET.Core/                     # [Shared Library] Dipakai Host & Agent
│   ├── NetworkServices.cs           # Modul Netsh, WMI, PowerShell
│   ├── PayloadModels.cs             # Model JSON (IP Request, Hostname Request)
│   └── LabProfile.cs                # Profil Lab
│
└── README.md
```

## 🚀 Alur Kerja / Workflow AirSET v2.0

1. **Instalasi:** `AirSET Agent` di-install dan dijalankan pada seluruh PC Lab (bisa di-include dalam *master image* Windows / cloning).
2. **Monitoring:** Admin membuka `AirSET Controller` di PC Dosen. Controller akan melakukan *broadcasting*.
3. **Handshake:** Seluruh PC Client merespon: *"Saya PC-LabA-01, IP 192.168.1.10, Online!"*.
4. **Action:** Admin memilih PC-LabA-01 sampai PC-LabA-40 di Dashboard, menginput format konfigurasi IP baru, dan menekan tombol **"Push Config"**.
5. **Execution:** Perintah (berbentuk JSON) dikirim ke masing-masing Client. `AirSET Agent` menerima perintah, melakukan eksekusi jaringan via `NetworkServices`, lalu merestart PC jika diperlukan.
6. **Result:** Dashboard Host menampilkan status `Success` (Hijau) pada PC yang berhasil dikonfigurasi.
