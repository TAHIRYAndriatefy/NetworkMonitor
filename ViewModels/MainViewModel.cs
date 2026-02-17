using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using NetworkMonitor.Models;
using NetworkMonitor.Services;
using NetworkMonitor.Utils;

namespace NetworkMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NetworkService _networkService;
        private readonly HostedNetworkService _hostedNetworkService;
        private readonly MACFilterService _macFilterService;
        private DispatcherTimer _updateTimer;

        private string _ssid;
        private double _downloadSpeed;
        private double _uploadSpeed;
        private int _connectedClients;
        private string _currentTime;
        private DeviceInfo _selectedDevice;
        private string _currentTheme = "Stylized";

        public ObservableCollection<DeviceInfo> Devices { get; }
        public ObservableCollection<string> Themes { get; }

        public ICommand RefreshCommand { get; }
        public ICommand BlockCommand { get; }
        public ICommand UnblockCommand { get; }

        public string SSID
        {
            get => _ssid;
            set { _ssid = value; OnPropertyChanged(); }
        }

        public double DownloadSpeed
        {
            get => _downloadSpeed;
            set { _downloadSpeed = value; OnPropertyChanged(); }
        }

        public double UploadSpeed
        {
            get => _uploadSpeed;
            set { _uploadSpeed = value; OnPropertyChanged(); }
        }

        public int ConnectedClients
        {
            get => _connectedClients;
            set { _connectedClients = value; OnPropertyChanged(); }
        }

        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(); }
        }

        public DeviceInfo SelectedDevice
        {
            get => _selectedDevice;
            set { _selectedDevice = value; OnPropertyChanged(); }
        }

        public string CurrentTheme
        {
            get => _currentTheme;
            set { _currentTheme = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _networkService = new NetworkService();
            _hostedNetworkService = new HostedNetworkService();
            _macFilterService = new MACFilterService();

            Devices = new ObservableCollection<DeviceInfo>();
            Themes = new ObservableCollection<string> { "Stylized", "Minimal", "Grand" };

            RefreshCommand = new RelayCommand(UpdateUI);
            BlockCommand = new RelayCommand(BlockSelectedDevice, () => SelectedDevice != null);
            UnblockCommand = new RelayCommand(UnblockSelectedDevice, () => SelectedDevice != null);

            InitializeTimer();
            UpdateUI();
        }

        private void InitializeTimer()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            _updateTimer.Tick += (s, e) => UpdateUI();
            _updateTimer.Start();
        }

        private void UpdateUI()
        {
            var stats = _networkService.GetNetworkStats();
            SSID = stats.SSID;
            DownloadSpeed = stats.DownloadKBps;
            UploadSpeed = stats.UploadKBps;
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var devices = _networkService.GetArpDevices();
            ConnectedClients = devices.Count;

            Devices.Clear();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
        }

        private void BlockSelectedDevice()
        {
            if (SelectedDevice != null)
            {
                _macFilterService.BlockMAC(SelectedDevice.MAC);
                SelectedDevice.IsBlocked = true;
            }
        }

        private void UnblockSelectedDevice()
        {
            if (SelectedDevice != null)
            {
                _macFilterService.UnblockMAC(SelectedDevice.MAC);
                SelectedDevice.IsBlocked = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
