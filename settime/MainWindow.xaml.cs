using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;

namespace SetTime
{
    public partial class MainWindow : Window
    {
        private readonly TimeZoneInfo MoscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        private readonly TimeZoneInfo EEuropeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
        private readonly TimeZoneInfo Gmt = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private TimeZoneInfo CurrentZone { get; set; }
        private NetworkTime networkTime { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            networkTime = new NetworkTime();

            var NtpTimer = new Timer(NtpClock, null, 0, 500);
            var Lbltimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), 
                DispatcherPriority.Normal, Lbltimer_Tick, Dispatcher);
            Lbltimer.Start();
        }
    
        private void TZcomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0: CurrentZone = Gmt; break;
                case 1: CurrentZone = MoscowZone; break;
                case 2: CurrentZone = EEuropeZone; break;
            }
            
            TimeZoneInfo.ConvertTimeFromUtc(networkTime.CurrentNtp, CurrentZone);
        }

        private void NtpClock(object state) => networkTime.CurrentNtp.AddMilliseconds(500);

        private void Lbltimer_Tick(object sender, EventArgs e)
        {
            SystemTime.Content = DateTime.Now.ToString("HH:mm:ss:ff");
            NtpTime.Content = networkTime.CurrentNtp.ToString("HH:mm:ss:ff");
            
            TimeSpan difference = DateTime.Now.Subtract(networkTime.CurrentNtp);
            Diff.Content = $"{difference.Seconds},{difference.Milliseconds}";
        }

        private void TimeManipulation_Click(object sender, RoutedEventArgs e)
        {
            string senderName = (sender as Button).Name;
            string textBoxName = senderName[0] == 's' ? "seconds" : "mseconds";
            var tempBox = FindName(textBoxName) as TextBox;
            var isDigit = Regex.IsMatch(tempBox.Text, @"^\d+$");

            if (isDigit == true)
            {
                UInt16 sec = UInt16.Parse(tempBox.Text);
                networkTime.TimeManipulation(senderName, sec);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0: CurrentZone = Gmt; break;
                case 1: CurrentZone = MoscowZone; break;
                case 2: CurrentZone = EEuropeZone; break;
            }          
            TimeZoneInfo.ConvertTimeFromUtc(networkTime.CurrentNtp, CurrentZone);
        }
    }
}
