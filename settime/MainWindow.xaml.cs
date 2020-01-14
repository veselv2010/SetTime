using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;

namespace SetTime
{
    public partial class MainWindow : Window
    {
        Timer NtpTimer = null;
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

        static void NtpClock(object state)
        {
            CurrentNtp = CurrentNtp.AddMilliseconds(500);
        }

        private void Lbltimer_Tick(object sender, EventArgs e)
        {
            SystemTime.Content = DateTime.Now.ToString("HH:mm:ss:ff");
            NtpTime.Content = CurrentNtp.ToString("HH:mm:ss:ff");
            
            TimeSpan difference = DateTime.Now.Subtract(CurrentNtp);
            Diff.Content = $"{difference.Seconds},{difference.Milliseconds}";
        }

        private void TimeManipulation(string type, string operation)
        {
            if (type == "seconds")
            {
                var isDigit = Regex.IsMatch(SecondsBox.Text, @"^\d+$");
                
                if (isDigit == true)
                {
                    
                    if (operation == "add")
                    {
                        var st = new NativeMethods.SYSTEMTIME();
                        NativeMethods.GetSystemTime(ref st);

                        var seconds = UInt16.Parse(SecondsBox.Text);
                        st.wSecond = (ushort)(st.wSecond + seconds % 60);

                        NativeMethods.SetSystemTime(ref st);
                    }
                    
                    else if (operation == "subract")
                    {
                        var st = new NativeMethods.SYSTEMTIME();
                        NativeMethods.GetSystemTime(ref st);

                        var seconds = UInt16.Parse(SecondsBox.Text);
                        st.wSecond = (ushort)(st.wSecond - seconds % 60);

                        NativeMethods.SetSystemTime(ref st);

                    }
                }

            }
            
            else if (type == "milliseconds")
            {
                var isDigit = Regex.IsMatch(MSecondsBox.Text, @"^\d+$");
                
                if (isDigit == true)
                {
                    if (operation == "add")
                    {
                        var st = new NativeMethods.SYSTEMTIME();
                        NativeMethods.GetSystemTime(ref st);

                        var mseconds = UInt16.Parse(MSecondsBox.Text);
                        st.wMilliseconds = (ushort)(st.wMilliseconds + mseconds % 60000);

                        NativeMethods.SetSystemTime(ref st);
                    }
                    
                    else if (operation == "subract")
                    {
                        var st = new NativeMethods.SYSTEMTIME();
                        NativeMethods.GetSystemTime(ref st);

                        var mseconds = UInt16.Parse(MSecondsBox.Text);
                        st.wMilliseconds = (ushort)(st.wMilliseconds - mseconds % 60000);

                        NativeMethods.SetSystemTime(ref st);

                    }

                }
            }
        }

        private void SecondsAdd_Click(object sender, RoutedEventArgs e)
        {
            TimeManipulation("seconds", "add");
        }

        private void MSecondsAdd_Click(object sender, RoutedEventArgs e)
        {
            TimeManipulation("milliseconds", "add");
        }

        private void SecondsSubt_Click(object sender, RoutedEventArgs e)
        {
            TimeManipulation("seconds", "subract");
        }

        private void MSecondsSubt_Click(object sender, RoutedEventArgs e)
        {
            TimeManipulation("milliseconds", "subract");
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
