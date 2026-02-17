using System.Windows;
using NetworkMonitor.ViewModels;

namespace NetworkMonitor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
