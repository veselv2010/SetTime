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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace settime
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

        public MainWindow()
        {
            InitializeComponent();
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
    }
}
