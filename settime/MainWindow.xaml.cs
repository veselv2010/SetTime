using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;

namespace SetTime
{
    public partial class MainWindow : Window
    {
        public struct SYSTEMTIME
        {
            public ushort wYear, wMonth, wDayOfWeek, wDay,
               wHour, wMinute, wSecond, wMilliseconds;
        }

        [DllImport("kernel32.dll")]
        public extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        static Timer NTPTimer = null;

        public MainWindow()
        {
            InitializeComponent();

            NTPTimer = new Timer( new TimerCallback(NTPClock), null, 0, 500);

            DispatcherTimer Lbltimer = new DispatcherTimer();
            Lbltimer.Interval = TimeSpan.FromMilliseconds(100);
            Lbltimer.Tick += Lbltimer_Tick;
            Lbltimer.Start();
        }

        static DateTime currentNTP = NetworkTime.GetNetworkTime();
        
        TimeZoneInfo 
        moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"),
        eeuropeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"),
        GMT = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"),
        currentZone;

        public void TZcomboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0:
                    currentZone = GMT;
                    break;

                case 1:
                    currentZone = moscowZone;
                    break;

                case 2:
                    currentZone = eeuropeZone;
                    break;
            }
            NetworkTime.GetNetworkTime();
            currentNTP = NetworkTime.GetNetworkTime();
            currentNTP = TimeZoneInfo.ConvertTimeFromUtc(currentNTP, currentZone);
        }

        static void NTPClock(object state)
        {
            currentNTP = currentNTP.AddMilliseconds(500);
        }

        public void Lbltimer_Tick(object sender, EventArgs e)
        {
            SystemTime.Content = DateTime.Now.ToString("HH:mm:ss:fff");
            NTPTime.Content = currentNTP.ToString("HH:mm:ss:fff");
            TimeSpan difference = DateTime.Now.Subtract(currentNTP);
            Diff.Content = $"{difference.Seconds},{difference.Milliseconds}";
        }

        public void secondsAdd_Click(object sender, RoutedEventArgs e)
        {
            var isDigit = Regex.IsMatch(secondsBox.Text, @"^\d+$");
            if (isDigit == true)
                {
                var st = new SYSTEMTIME();
                GetSystemTime(ref st);

                var seconds = UInt16.Parse(secondsBox.Text);
                st.wSecond = (ushort)(st.wSecond + seconds % 60);

                SetSystemTime(ref st);
                }
        }

        public void msecondsAdd_Click(object sender, RoutedEventArgs e)
        {
            var isDigit = Regex.IsMatch(secondsBox.Text, @"^\d+$");
            if (isDigit == true)
            {
                var st = new SYSTEMTIME();
                GetSystemTime(ref st);

                var mseconds = UInt16.Parse(msecondsBox.Text);
                st.wMilliseconds = (ushort)(st.wMilliseconds + mseconds % 60000);

                SetSystemTime(ref st);
            }
        }

        public void secondsSubt_Click(object sender, RoutedEventArgs e)
        {
            var isDigit = Regex.IsMatch(secondsBox.Text, @"^\d+$");
            if (isDigit == true)
            {
                var st = new SYSTEMTIME();
                GetSystemTime(ref st);

                var seconds = UInt16.Parse(secondsBox.Text);
                st.wSecond = (ushort)(st.wSecond - seconds % 60);

                SetSystemTime(ref st);
            }
        }

        public void msecondsSubt_Click(object sender, RoutedEventArgs e)
        {
            var isDigit = Regex.IsMatch(secondsBox.Text, @"^\d+$");
            if (isDigit == true)
            {
                var st = new SYSTEMTIME();
                GetSystemTime(ref st);

                var mseconds = UInt16.Parse(msecondsBox.Text);
                st.wMilliseconds = (ushort)(st.wMilliseconds - mseconds % 60000);

                SetSystemTime(ref st);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (TZcomboBox.SelectedIndex)
            {
                case 0:
                    currentZone = GMT;
                    break;

                case 1:
                    currentZone = moscowZone;
                    break;

                case 2:
                    currentZone = eeuropeZone;
                    break;
            }
            NetworkTime.GetNetworkTime();
            currentNTP = NetworkTime.GetNetworkTime();
            currentNTP = TimeZoneInfo.ConvertTimeFromUtc(currentNTP, currentZone);
        }
    }
}
