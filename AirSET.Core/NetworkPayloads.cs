using System;
using System.Collections.Generic;

namespace AirSET.Core.Models
{
    public static class NetworkConstants
    {
        public const int DefaultPort = 3623;
        public const string ClusterAuthHeader = "AIRSET_V2_AUTH_KEY";
    }

    public class DiscoveryMessage
    {
        public string Type { get; set; } // "DISCOVERY_REQUEST" | "DISCOVERY_RESPONSE"
        public string Hostname { get; set; }
        public string MacAddress { get; set; }
        public string CurrentIp { get; set; }
        public string Subnet { get; set; }
        public string Gateway { get; set; }
        public string Workgroup { get; set; }
        public string Status { get; set; } // "Ready", "Busy", "Online"
        public string CurrentAdapter { get; set; }
        public string Version { get; set; }
        public string OsCaption { get; set; }
        public string ShieldStatus { get; set; }

        public DiscoveryMessage()
        {
            Type = "DISCOVERY_RESPONSE";
            Version = "2.0";
            Status = "Online";
            OsCaption = "Windows";
            ShieldStatus = "Unknown";
        }
    }

    public class RemoteConfigPayload
    {
        public string Action { get; set; } // "APPLY_CONFIG", "REBOOT", "SHUTDOWN", "SHIELD_ACTION", "POWER_SHUTDOWN", "POWER_REBOOT", "CLEAR_DESKTOP", "QUICK_LAUNCH", "FILE_TRANSFER", "COLLECT_WORK"
        public int PcNumber { get; set; }
        public string BaseIp { get; set; }
        public string Subnet { get; set; }
        public string Gateway { get; set; }
        public string Dns { get; set; }
        public string Prefix { get; set; }
        public string Workgroup { get; set; }
        public string TargetInterface { get; set; }

        public bool ChangePassword { get; set; }
        public string NewPassword { get; set; }
        public bool AutoLogon { get; set; }
        public bool DisableAutoLogon { get; set; }
        public bool RestartAfterApply { get; set; }

        // AirSET Hybrid Disk Shield
        public string ShieldCommand { get; set; } // "UWF_INSTALL", "UWF_UNINSTALL", "UWF_LOCK", "UWF_UNLOCK", "DFC_FREEZE", "DFC_THAW"
        public string DeepFreezePassword { get; set; }

        // Quick Launch
        public string LaunchTarget { get; set; } // Path exe, nama aplikasi (chrome, cmd), atau URL
        public string LaunchArguments { get; set; }

        // File Transfer & Distribution
        public string FileName { get; set; }
        public string TargetDirectory { get; set; } // "Desktop", "Downloads", "C:\Praktikum", dll
        public string FileDataBase64 { get; set; }
        public long FileSize { get; set; }

        // Collect Work
        public string CollectSourceFolder { get; set; } // Folder di client yang mau ditarik ("Desktop", "Documents", "Downloads", atau path absolut)
        public string CollectFilePattern { get; set; }  // Pattern file ("*.txt", "*.docx", "*.*", dll)
        public bool DeleteAfterCollect { get; set; }    // Hapus file di client setelah berhasil dikumpulkan

        // Remote PowerShell Scripting
        public string ScriptContent { get; set; }

        // Wallpaper Changer
        public string WallpaperDataBase64 { get; set; }
        public string WallpaperStyle { get; set; } // "Fill", "Fit", "Stretch", "Tile", "Center"

        // Interactive Remote Control (NetSupport / Remote Desktop Style)
        public string InputEventType { get; set; } // "MOUSE_MOVE", "MOUSE_DOWN", "MOUSE_UP", "KEY_DOWN", "KEY_UP", "TEXT_INPUT", "MOUSE_WHEEL"
        public int InputX { get; set; }
        public int InputY { get; set; }
        public string InputButton { get; set; } // "Left", "Right", "Middle"
        public int InputKey { get; set; }       // Virtual-Key Code
        public string InputText { get; set; }   // Karakter teks langsung
        public int InputWheelDelta { get; set; } // Nilai scroll roda mouse

        // Screen Capture Options
        public int ScreenQuality { get; set; } // 10 - 100
        public int TargetWidth { get; set; }   // 0 = Native
        public int TargetHeight { get; set; }  // 0 = Native
    }

    public class HardwareInfo
    {
        public string MachineName { get; set; }
        public string Processor { get; set; }
        public string Ram { get; set; }
        public string Motherboard { get; set; }
        public string Storage { get; set; }
        public string Graphics { get; set; }
        public string OsName { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
    }

    public class SoftwareInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string InstallDate { get; set; }
    }

    public class ClientInventoryData
    {
        public HardwareInfo Hardware { get; set; }
        public List<SoftwareInfo> SoftwareList { get; set; }

        public ClientInventoryData()
        {
            Hardware = new HardwareInfo();
            SoftwareList = new List<SoftwareInfo>();
        }
    }

    public class ExecutionResponse
    {
        public bool Success { get; set; }
        public string Hostname { get; set; }
        public string Message { get; set; }
        public string OutputLog { get; set; }
        public string FileDataResponseBase64 { get; set; } // Data zip hasil collect pekerjaan siswa
        public string ResultFileName { get; set; }
        public string InventoryDataJson { get; set; } // Data Hardware & Software terinstall
        public string ScreenThumbnailBase64 { get; set; } // Thumbnail tangkapan layar PC client (JPEG Base64)
        public int ClientScreenWidth { get; set; }  // Lebar layar asli client (pixel)
        public int ClientScreenHeight { get; set; } // Tinggi layar asli client (pixel)

        public ExecutionResponse()
        {
            Success = false;
            Message = string.Empty;
            OutputLog = string.Empty;
            FileDataResponseBase64 = string.Empty;
            ResultFileName = string.Empty;
            InventoryDataJson = string.Empty;
            ScreenThumbnailBase64 = string.Empty;
            ClientScreenWidth = 0;
            ClientScreenHeight = 0;
        }
    }
}
