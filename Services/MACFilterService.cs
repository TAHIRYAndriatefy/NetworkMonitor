using System;
using System.Diagnostics;

namespace NetworkMonitor.Services
{
    public class MACFilterService
    {
        public void BlockMAC(string mac)
        {
            try
            {
                var process = new ProcessStartInfo("netsh", 
                    $"wlan add filter permission=block mac={mac} networktype=infrastructure")
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process.Start(process))
                {
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors du blocage: {ex.Message}");
            }
        }

        public void UnblockMAC(string mac)
        {
            try
            {
                var process = new ProcessStartInfo("netsh", 
                    $"wlan delete filter permission=block mac={mac} networktype=infrastructure")
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (Process.Start(process))
                {
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors du déblocage: {ex.Message}");
            }
        }
    }
}
