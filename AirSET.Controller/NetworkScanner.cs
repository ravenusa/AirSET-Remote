using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AirSET.Core.Models;
using AirSET.Core.Services;

namespace AirSET.Controller
{
    public class NetworkScanner
    {
        public event Action<DiscoveryMessage, string> OnDeviceDiscovered;
        public event Action<string> OnLog;

        public async Task BroadcastDiscoveryAsync(int timeoutMs = 2500)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.EnableBroadcast = true;
                    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    // Fix Windows Winsock bug: Abaikan ICMP Port Unreachable (WSAECONNRESET 10054)
                    const int SIO_UDP_CONNRESET = -1744830452;
                    try
                    {
                        byte[] inVal = new byte[] { 0 };
                        byte[] outVal = new byte[] { 0 };
                        udpClient.Client.IOControl(SIO_UDP_CONNRESET, inVal, outVal);
                    }
                    catch { }

                    // Tingkatkan buffer receive UDP agar tidak terjadi overflow saat 70+ client membalas serentak
                    try { udpClient.Client.ReceiveBufferSize = 1024 * 1024; } catch { }

                    string discoveryMsg = "{\"Type\":\"DISCOVERY_REQUEST\",\"Version\":\"2.0\"}";
                    byte[] bytes = Encoding.UTF8.GetBytes(discoveryMsg);

                    // Helper lokal untuk menyebarkan paket discovery ke broadcast global, loopback, dan semua adapter
                    Func<Task> sendDiscoveryBursts = async () =>
                    {
                        try { await udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, NetworkConstants.DefaultPort)); } catch { }
                        try { await udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, NetworkConstants.DefaultPort)); } catch { }

                        foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                        {
                            if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                            {
                                foreach (var u in ni.GetIPProperties().UnicastAddresses)
                                {
                                    if (u.Address.AddressFamily == AddressFamily.InterNetwork && u.IPv4Mask != null)
                                    {
                                        try
                                        {
                                            byte[] ipBytes = u.Address.GetAddressBytes();
                                            byte[] maskBytes = u.IPv4Mask.GetAddressBytes();
                                            byte[] broadcastBytes = new byte[ipBytes.Length];
                                            for (int i = 0; i < ipBytes.Length; i++)
                                            {
                                                broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
                                            }
                                            IPAddress bcast = new IPAddress(broadcastBytes);
                                            await udpClient.SendAsync(bytes, bytes.Length, new IPEndPoint(bcast, NetworkConstants.DefaultPort));
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    };

                    // Gelombang 1: Kirim broadcast pertama
                    await sendDiscoveryBursts();
                    OnLog?.Invoke(string.Format("[{0}] Mengirim UDP Discovery broadcast (Gelombang 1) ke port {1}...", DateTime.Now.ToString("HH:mm:ss"), NetworkConstants.DefaultPort));

                    // Gelombang 2: Kirim ulang broadcast 750ms kemudian di latar belakang untuk menangkap paket yang dropped di switch
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(750);
                        try
                        {
                            await sendDiscoveryBursts();
                        }
                        catch { }
                    });

                    // Menerima balasan dari client secara agresif
                    DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMs);
                    while (DateTime.Now < endTime)
                    {
                        int remainingMs = (int)(endTime - DateTime.Now).TotalMilliseconds;
                        if (remainingMs <= 0) break;

                        try
                        {
                            var receiveTask = udpClient.ReceiveAsync();
                            var delayTask = Task.Delay(remainingMs);

                            var completed = await Task.WhenAny(receiveTask, delayTask);
                            if (completed == receiveTask)
                            {
                                var res = receiveTask.Result;
                                string json = Encoding.UTF8.GetString(res.Buffer);
                                var serializer = new JavaScriptSerializer();
                                var msg = serializer.Deserialize<DiscoveryMessage>(json);
                                if (msg != null && msg.Type == "DISCOVERY_RESPONSE")
                                {
                                    string clientIp = res.RemoteEndPoint.Address.ToString();
                                    if (clientIp == "127.0.0.1" && !string.IsNullOrEmpty(msg.CurrentIp))
                                    {
                                        clientIp = msg.CurrentIp;
                                    }
                                    OnDeviceDiscovered?.Invoke(msg, clientIp);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (SocketException)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke(string.Format("[{0}] Broadcast Error: {1}", DateTime.Now.ToString("HH:mm:ss"), ex.Message));
            }
        }

        public async Task ScanSubnetRangeAsync(string ipRangeOrBase = null, int probeTimeoutMs = 600, int maxDegreeOfParallelism = 50)
        {
            try
            {
                List<string> candidateIps = GetTargetIpCandidates(ipRangeOrBase);
                if (candidateIps == null || candidateIps.Count == 0)
                {
                    OnLog?.Invoke(string.Format("[{0}] Tidak ada range subnet yang valid untuk di-scan.", DateTime.Now.ToString("HH:mm:ss")));
                    return;
                }

                OnLog?.Invoke(string.Format("[{0}] Memulai Parallel Fast-Scan ke {1} IP subnet (Port {2})...", 
                    DateTime.Now.ToString("HH:mm:ss"), candidateIps.Count, NetworkConstants.DefaultPort));

                using (var semaphore = new SemaphoreSlim(maxDegreeOfParallelism))
                {
                    var tasks = new List<Task>();
                    foreach (var ip in candidateIps)
                    {
                        await semaphore.WaitAsync();
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await ProbeIpAsync(ip, probeTimeoutMs);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                }

                OnLog?.Invoke(string.Format("[{0}] Selesai fast-scan subnet.", DateTime.Now.ToString("HH:mm:ss")));
            }
            catch (Exception ex)
            {
                OnLog?.Invoke(string.Format("[{0}] Subnet Scan Error: {1}", DateTime.Now.ToString("HH:mm:ss"), ex.Message));
            }
        }

        private async Task ProbeIpAsync(string ip, int timeoutMs)
        {
            try
            {
                using (var tcp = new TcpClient())
                {
                    var connectTask = tcp.ConnectAsync(ip, NetworkConstants.DefaultPort);
                    if (await Task.WhenAny(connectTask, Task.Delay(timeoutMs)) != connectTask)
                    {
                        return; // Timeout / offline
                    }
                    if (!tcp.Connected) return;

                    // Port 3623 terbuka! Kirim PING_DISCOVERY handshake
                    var payload = new RemoteConfigPayload { Action = "PING_DISCOVERY" };
                    string encryptedPayload = CryptoManager.Encrypt(payload);
                    byte[] data = Encoding.UTF8.GetBytes(encryptedPayload);

                    using (var stream = tcp.GetStream())
                    {
                        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                        byte[] packet = new byte[lengthBytes.Length + data.Length];
                        Buffer.BlockCopy(lengthBytes, 0, packet, 0, lengthBytes.Length);
                        Buffer.BlockCopy(data, 0, packet, lengthBytes.Length, data.Length);

                        await stream.WriteAsync(packet, 0, packet.Length);
                        await stream.FlushAsync();

                        byte[] buffer = new byte[4096];
                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                        if (await Task.WhenAny(readTask, Task.Delay(timeoutMs)) == readTask)
                        {
                            int bytesRead = await readTask;
                            if (bytesRead > 0)
                            {
                                string rawResp = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                var serializer = new JavaScriptSerializer();
                                var execResp = serializer.Deserialize<ExecutionResponse>(rawResp);
                                string hostname = (execResp != null && !string.IsNullOrEmpty(execResp.Hostname)) ? execResp.Hostname : ip;

                                string osCap = "Windows";
                                string shieldStat = "Unknown";
                                string macAddr = "";
                                if (execResp != null && !string.IsNullOrEmpty(execResp.OutputLog) && execResp.OutputLog.Contains("|"))
                                {
                                    var parts = execResp.OutputLog.Split('|');
                                    if (parts.Length >= 1) osCap = parts[0];
                                    if (parts.Length >= 2) shieldStat = parts[1];
                                    if (parts.Length >= 3) macAddr = parts[2];
                                }

                                var msg = new DiscoveryMessage
                                {
                                    Type = "DISCOVERY_RESPONSE",
                                    Hostname = hostname,
                                    MacAddress = macAddr,
                                    CurrentIp = ip,
                                    Status = "Online",
                                    OsCaption = osCap,
                                    ShieldStatus = shieldStat
                                };
                                OnDeviceDiscovered?.Invoke(msg, ip);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently skip non-responsive hosts
            }
        }

        private List<string> GetTargetIpCandidates(string specifiedBase)
        {
            var result = new HashSet<string>();

            // Jika ada specifiedBase (misal "192.168.10" atau "192.168.11")
            if (!string.IsNullOrEmpty(specifiedBase) && specifiedBase.Split('.').Length == 3)
            {
                for (int i = 1; i <= 254; i++)
                {
                    result.Add(string.Format("{0}.{1}", specifiedBase, i));
                }
            }

            // Dapatkan seluruh IP subnet lokal dari semua network adapter aktif
            foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up ||
                    ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var u in ni.GetIPProperties().UnicastAddresses)
                {
                    if (u.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string localIp = u.Address.ToString();
                        if (localIp.StartsWith("169.254")) continue;

                        string[] parts = localIp.Split('.');
                        if (parts.Length == 4)
                        {
                            string prefix24 = string.Format("{0}.{1}.{2}", parts[0], parts[1], parts[2]);
                            for (int i = 1; i <= 254; i++)
                            {
                                result.Add(string.Format("{0}.{1}", prefix24, i));
                            }

                            // Jika netmask /23 (seperti 192.168.11.x dan 192.168.10.x), masukkan juga blok pasangan
                            if (u.IPv4Mask != null && u.IPv4Mask.ToString() == "255.255.254.0")
                            {
                                int thirdOctet = int.Parse(parts[2]);
                                int pairOctet = (thirdOctet % 2 == 0) ? thirdOctet + 1 : thirdOctet - 1;
                                string pairPrefix = string.Format("{0}.{1}.{2}", parts[0], parts[1], pairOctet);
                                for (int i = 1; i <= 254; i++)
                                {
                                    result.Add(string.Format("{0}.{1}", pairPrefix, i));
                                }
                            }
                        }
                    }
                }
            }

            return new List<string>(result);
        }

        public async Task ScanDirectIpAsync(string targetIp, int timeoutMs = 3500)
        {
            try
            {
                OnLog?.Invoke(string.Format("[{0}] Memeriksa koneksi langsung ke {1}:{2}...", DateTime.Now.ToString("HH:mm:ss"), targetIp, NetworkConstants.DefaultPort));
                var payload = new RemoteConfigPayload { Action = "PING_DISCOVERY" };
                var resp = await SendRemoteConfigAsync(targetIp, payload, timeoutMs);

                if (resp != null && resp.Success)
                {
                    string osCap = "Windows";
                    string shieldStat = "Unknown";
                    if (!string.IsNullOrEmpty(resp.OutputLog) && resp.OutputLog.Contains("|"))
                    {
                        var parts = resp.OutputLog.Split('|');
                        if (parts.Length >= 2)
                        {
                            osCap = parts[0];
                            shieldStat = parts[1];
                        }
                    }

                    var msg = new DiscoveryMessage
                    {
                        Type = "DISCOVERY_RESPONSE",
                        Hostname = string.IsNullOrEmpty(resp.Hostname) ? targetIp : resp.Hostname,
                        CurrentIp = targetIp,
                        Status = "Online",
                        OsCaption = osCap,
                        ShieldStatus = shieldStat
                    };
                    OnDeviceDiscovered?.Invoke(msg, targetIp);
                    OnLog?.Invoke(string.Format("[{0}] ✅ Berhasil mendeteksi Client {1} ({2}) [{3} | {4}]", DateTime.Now.ToString("HH:mm:ss"), msg.Hostname, targetIp, osCap, shieldStat));
                }
                else
                {
                    string err = resp != null ? resp.Message : "Tidak ada balasan";
                    OnLog?.Invoke(string.Format("[{0}] ❌ {1} tidak merespon: {2}", DateTime.Now.ToString("HH:mm:ss"), targetIp, err));
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke(string.Format("[{0}] Error direct scan {1}: {2}", DateTime.Now.ToString("HH:mm:ss"), targetIp, ex.Message));
            }
        }

        public async Task<ExecutionResponse> SendRemoteConfigAsync(string targetIp, RemoteConfigPayload payload, int timeoutMs = 15000)
        {
            var response = new ExecutionResponse { Hostname = targetIp };
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(targetIp, NetworkConstants.DefaultPort);
                    var delayTask = Task.Delay(timeoutMs);

                    if (await Task.WhenAny(connectTask, delayTask) == delayTask)
                    {
                        response.Success = false;
                        response.Message = "Timeout: Komputer Client tidak merespon koneksi TCP.";
                        return response;
                    }

                    string encryptedPayload = CryptoManager.Encrypt(payload);
                    byte[] data = Encoding.UTF8.GetBytes(encryptedPayload);

                    using (NetworkStream stream = client.GetStream())
                    {
                        // Kirim length prefix (4 bytes)
                        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
                        await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                        // Kirim data payload secara chunked (64 KB per blok) untuk mencegah OutOfMemory / Socket Reset
                        int offset = 0;
                        int chunkSize = 65536; // 64 KB
                        while (offset < data.Length)
                        {
                            int bytesToSend = Math.Min(chunkSize, data.Length - offset);
                            await stream.WriteAsync(data, offset, bytesToSend);
                            offset += bytesToSend;
                        }
                        await stream.FlushAsync();

                        // Baca balasan respon dari client
                        // Cek apakah balasan diawali dengan 4-byte length prefix
                        byte[] lenBuffer = new byte[4];
                        int lenBytesRead = 0;
                        while (lenBytesRead < 4)
                        {
                            var rTask = stream.ReadAsync(lenBuffer, lenBytesRead, 4 - lenBytesRead);
                            var dTask = Task.Delay(timeoutMs);
                            if (await Task.WhenAny(rTask, dTask) == dTask)
                            {
                                response.Success = false;
                                response.Message = "Timeout saat menunggu balasan respon Client.";
                                return response;
                            }
                            int read = await rTask;
                            if (read <= 0) break;
                            lenBytesRead += read;
                        }

                        if (lenBytesRead == 0)
                        {
                            response.Success = false;
                            response.Message = "Client menutup koneksi tanpa mengirim respon.";
                            return response;
                        }

                        using (var ms = new System.IO.MemoryStream())
                        {
                            if (lenBytesRead == 4)
                            {
                                int expectedLen = BitConverter.ToInt32(lenBuffer, 0);
                                // Validasi jika 4 byte tersebut memang panjang (panjang wajar hingga 500MB)
                                if (expectedLen > 0 && expectedLen < 500 * 1024 * 1024)
                                {
                                    byte[] chunk = new byte[65536];
                                    int totalRead = 0;
                                    while (totalRead < expectedLen)
                                    {
                                        int toRead = Math.Min(chunk.Length, expectedLen - totalRead);
                                        var rTask = stream.ReadAsync(chunk, 0, toRead);
                                        var dTask = Task.Delay(timeoutMs);
                                        if (await Task.WhenAny(rTask, dTask) == dTask)
                                        {
                                            response.Success = false;
                                            response.Message = "Timeout saat mendownload data dari Client.";
                                            return response;
                                        }
                                        int read = await rTask;
                                        if (read <= 0) break;
                                        ms.Write(chunk, 0, read);
                                        totalRead += read;
                                    }
                                }
                                else
                                {
                                    // Bukan length prefix standar (misal JSON raw stream), tulis 4 byte pertama ke ms
                                    ms.Write(lenBuffer, 0, 4);
                                    byte[] buffer = new byte[16384];
                                    while (true)
                                    {
                                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                                        var readDelay = Task.Delay(timeoutMs);
                                        if (await Task.WhenAny(readTask, readDelay) == readDelay) break;
                                        int bytesRead = await readTask;
                                        if (bytesRead <= 0) break;
                                        ms.Write(buffer, 0, bytesRead);
                                        if (!stream.DataAvailable)
                                        {
                                            await Task.Delay(100);
                                            if (!stream.DataAvailable) break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ms.Write(lenBuffer, 0, lenBytesRead);
                            }

                            if (ms.Length == 0)
                            {
                                response.Success = false;
                                response.Message = "Tidak ada data respon yang diterima dari Client.";
                                return response;
                            }

                            string rawResp = Encoding.UTF8.GetString(ms.ToArray());
                            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                            var execResp = serializer.Deserialize<ExecutionResponse>(rawResp);
                            return execResp ?? new ExecutionResponse { Success = true, Message = "Konfigurasi terkirim." };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }

        /// <summary>
        /// Mengirimkan Wake-on-LAN Magic Packet (UDP Broadcast Port 9) untuk menyalakan PC dari kondisi mati.
        /// </summary>
        public async Task<bool> SendWakeOnLanAsync(string macAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(macAddress)) return false;
                string cleanMac = macAddress.Replace(":", "").Replace("-", "").Replace(".", "").Trim();
                if (cleanMac.Length != 12) return false;

                byte[] macBytes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = Convert.ToByte(cleanMac.Substring(i * 2, 2), 16);
                }

                // Format Magic Packet: 6 byte 0xFF diikuti 16 repetisi MAC address (Total 102 byte)
                byte[] packet = new byte[6 + 16 * 6];
                for (int i = 0; i < 6; i++) packet[i] = 0xFF;
                for (int i = 0; i < 16; i++)
                {
                    Buffer.BlockCopy(macBytes, 0, packet, 6 + i * 6, 6);
                }

                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    await client.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 9));
                    await client.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 7));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}