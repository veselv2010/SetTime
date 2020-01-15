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
        Timer NtpTimer { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            NtpTimer = new Timer( new TimerCallback(NtpClock), null, 0, 500);

            var Lbltimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            Lbltimer.Tick += Lbltimer_Tick;
            Lbltimer.Start();
        }

        static DateTime CurrentNtp = NetworkTime.GetNetworkTime();
        
        private readonly TimeZoneInfo MoscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        private readonly TimeZoneInfo EEuropeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
        private readonly TimeZoneInfo Gmt = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private TimeZoneInfo CurrentZone;

        private void TZcomboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0:
                    CurrentZone = Gmt;
                    break;

                case 1:
                    CurrentZone = MoscowZone;
                    break;

                case 2:
                    CurrentZone = EEuropeZone;
                    break;
            }         
            NetworkTime.GetNetworkTime();
            
            CurrentNtp = NetworkTime.GetNetworkTime();
            CurrentNtp = TimeZoneInfo.ConvertTimeFromUtc(CurrentNtp, CurrentZone);
        }

        static void NtpClock(object state) => CurrentNtp = CurrentNtp.AddMilliseconds(500);

        private void Lbltimer_Tick(object sender, EventArgs e)
        {
            SystemTime.Content = DateTime.Now.ToString("HH:mm:ss:ff");
            NtpTime.Content = CurrentNtp.ToString("HH:mm:ss:ff");
            
            TimeSpan difference = DateTime.Now.Subtract(CurrentNtp);
            Diff.Content = $"{difference.Seconds},{difference.Milliseconds}";
        }

        private void TimeManipulation(string senderName)
        {
            char operation = senderName[senderName.Length - 1];
            string textBoxName = senderName[0] == 's' ? "seconds" : "mseconds";
            var tempBox = FindName(textBoxName) as TextBox;
            var isDigit = Regex.IsMatch(tempBox.Text, @"^\d+$");

            if (isDigit == true)
            {
                var st = new NativeMethods.SYSTEMTIME();
                NativeMethods.GetSystemTime(ref st);

                var sec = UInt16.Parse(tempBox.Text);

                if (senderName.StartsWith("s"))
                {
                    switch (operation)
                    {
                        case 'A': st.wSecond = (ushort)(st.wSecond + sec % 60); break;
                        case 'S': st.wSecond = (ushort)(st.wSecond - sec % 60); break;
                    };
                }
                else
                {
                    switch (operation)
                    {
                        case 'A': st.wMilliseconds = (ushort)(st.wMilliseconds + sec % 60000); break;
                        case 'S': st.wMilliseconds = (ushort)(st.wMilliseconds - sec % 60000); break;
                    };
                }
                NativeMethods.SetSystemTime(ref st);
            }
            else
                return;           
        }

        private void TimeManipulation_Click(object sender, RoutedEventArgs e)
        {
            string senderName = (sender as Button).Name;
            TimeManipulation(senderName);
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0:
                    CurrentZone = Gmt;
                    break;

                case 1:
                    CurrentZone = MoscowZone;
                    break;

                case 2:
                    CurrentZone = EEuropeZone;
                    break;
            }
            NetworkTime.GetNetworkTime();
            
            CurrentNtp = NetworkTime.GetNetworkTime();
            CurrentNtp = TimeZoneInfo.ConvertTimeFromUtc(CurrentNtp, CurrentZone);
        }
    }
}
