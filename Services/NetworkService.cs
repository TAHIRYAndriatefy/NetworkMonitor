using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using NetworkMonitor.Models;

namespace NetworkMonitor.Services
{
    public class NetworkService
    {
        private NetworkInterface _currentInterface;
        private long _previousRxBytes;
        private long _previousTxBytes;
        private DateTime _lastUpdateTime;

        public NetworkService()
        {
            InitializeInterface();
            _lastUpdateTime = DateTime.Now;
        }

        private void InitializeInterface()
        {
            _currentInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless &&
                                    x.OperationalStatus == OperationalStatus.Up);
        }

        public string GetWifiSSID()
        {
            try
            {
                var process = new ProcessStartInfo("netsh", "wlan show interfaces")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(process))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    var match = Regex.Match(output, @"SSID\s*:\s*(.+?)$", RegexOptions.Multiline);
                    return match.Success ? match.Groups[1].Value.Trim() : "(non connecté)";
                }
            }
            catch
            {
                return "(non connecté)";
            }
        }

        public NetworkStats GetNetworkStats()
        {
            var stats = new NetworkStats
            {
                SSID = GetWifiSSID(),
                ConnectedClients = 0
            };

            if (_currentInterface != null)
            {
                var interfaceStats = _currentInterface.GetIPStatistics();
                var now = DateTime.Now;
                var delta = (now - _lastUpdateTime).TotalSeconds;

                if (delta > 0)
                {
                    var rxDelta = interfaceStats.BytesReceived - _previousRxBytes;
                    var txDelta = interfaceStats.BytesSent - _previousTxBytes;

                    stats.DownloadKBps = Math.Round((rxDelta / 1024.0) / delta, 2);
                    stats.UploadKBps = Math.Round((txDelta / 1024.0) / delta, 2);
                }

                _previousRxBytes = interfaceStats.BytesReceived;
                _previousTxBytes = interfaceStats.BytesSent;
                _lastUpdateTime = now;
            }

            return stats;
        }

        public List<DeviceInfo> GetArpDevices()
        {
            var devices = new List<DeviceInfo>();

            try
            {
                var process = new ProcessStartInfo("arp", "-a")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(process))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    var matches = Regex.Matches(output, @"^\s*([\d\.]+)\s+([0-9a-f:-]{17})", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    foreach (Match match in matches)
                    {
                        string ip = match.Groups[1].Value;
                        string mac = match.Groups[2].Value.ToUpper();

                        string hostname = GetHostName(ip);

                        devices.Add(new DeviceInfo
                        {
                            IP = ip,
                            MAC = mac,
                            Name = hostname,
                            Time = DateTime.Now.ToString("HH:mm:ss")
                        });
                    }
                }
            }
            catch { }

            return devices;
        }

        private string GetHostName(string ip)
        {
            try
            {
                var entry = Dns.GetHostEntry(ip);
                return entry?.HostName ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
