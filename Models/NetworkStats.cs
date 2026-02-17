namespace NetworkMonitor.Models
{
    public class NetworkStats
    {
        public string SSID { get; set; }
        public double DownloadKBps { get; set; }
        public double UploadKBps { get; set; }
        public int ConnectedClients { get; set; }
    }
}
