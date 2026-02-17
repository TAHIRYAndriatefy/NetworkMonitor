using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkMonitor.Models
{
    public class DeviceInfo : INotifyPropertyChanged
    {
        private string _name;
        private string _ip;
        private string _mac;
        private string _time;
        private bool _isBlocked;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string IP
        {
            get => _ip;
            set { _ip = value; OnPropertyChanged(); }
        }

        public string MAC
        {
            get => _mac;
            set { _mac = value; OnPropertyChanged(); }
        }

        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(); }
        }

        public bool IsBlocked
        {
            get => _isBlocked;
            set { _isBlocked = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
