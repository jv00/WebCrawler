using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebCrawler.DesktopUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int MaxNumberOfMonitorLines = 50;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cr = new CrawlerEntryPoint();
            string baseUrl = UrlTextbox.Text.Trim();
            cr.InitializeAndStartCrawler(UpdateProcessingMonitor, baseUrl);
        }

        public void UpdateProcessingMonitor(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var currentLogs = ScrapingMonitorTextBlock.Text.Split('\n').ToList();

                if (currentLogs.Count == MaxNumberOfMonitorLines)
                {
                    currentLogs.RemoveAt(currentLogs.Count - 1);
                }

                var newLogs = new List<string>() { message };
                newLogs.AddRange(currentLogs);
                ScrapingMonitorTextBlock.Text = string.Join<string>("\n", newLogs);
            });
        }
    }
}
