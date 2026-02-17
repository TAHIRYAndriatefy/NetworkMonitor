using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NetworkMonitor.Models;

namespace NetworkMonitor.Services
{
    public class HostedNetworkService
    {
        public List<DeviceInfo> GetHostedNetworkClients()
        {
            var clients = new List<DeviceInfo>();

            try
            {
                var process = new ProcessStartInfo("netsh", "wlan show hostednetwork")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(process))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    var matches = Regex.Matches(output, @"([0-9A-Fa-f:-]{17})", RegexOptions.IgnoreCase);

                    foreach (Match match in matches)
                    {
                        clients.Add(new DeviceInfo
                        {
                            MAC = match.Groups[1].Value.ToUpper(),
                            IP = string.Empty,
                            Name = string.Empty,
                            Time = DateTime.Now.ToString("HH:mm:ss")
                        });
                    }
                }
            }
            catch { }

            return clients;
        }
    }
}
