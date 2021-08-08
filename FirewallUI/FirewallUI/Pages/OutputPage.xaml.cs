using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SerialManager;
using Windows.UI;
using CodeManager;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirewallUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutputPage : Page
    {

        SerialMonitor monitor = new SerialMonitor();

        private bool IsP0Correct = false;
        private bool IsP1Correct = false;
        private bool IsP2Correct = false;
        private bool IsP3Correct = false;

        private readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        private readonly SolidColorBrush NormalBrush = new SolidColorBrush(Color.FromArgb(255, 99, 99, 99));

        private bool CanSendRequest => IsP0Correct && IsP1Correct && IsP2Correct && IsP3Correct;
        public OutputPage()
        {
            this.InitializeComponent();
            var port = monitor.FindPort();
            if (port == "NOPORT")
            {
                DisableButtons();
                
            }    
            else
            {
                if (CanSendRequest)
                    EnableButons();
                else
                    DisableButtons();
            }
        }

        private void DisableButtons()
        {
            CheckIncoming.IsEnabled = false;
            CheckOutgoin.IsEnabled = false;
        }

        private void EnableButons()
        {
            CheckOutgoin.IsEnabled = true;
            CheckIncoming.IsEnabled = true;
            monitor.FindPort();
        }

        private void P0_TextChanged(object sender, TextChangedEventArgs e)
        {
            P0.BorderBrush = (IsP0Correct = Byte.TryParse(P0.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();

        }


        private void P1_TextChanged(object sender, TextChangedEventArgs e)
        {
            P1.BorderBrush = (IsP1Correct = Byte.TryParse(P1.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void P2_TextChanged(object sender, TextChangedEventArgs e)
        {
            P2.BorderBrush = (IsP2Correct = Byte.TryParse(P2.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void P3_TextChanged(object sender, TextChangedEventArgs e)
        {
            P3.BorderBrush = (IsP3Correct = Byte.TryParse(P3.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void CheckIncoming_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            byte p0 = Byte.Parse(P0.Text);
            byte p1 = Byte.Parse(P1.Text);
            byte p2 = Byte.Parse(P2.Text);
            byte p3 = Byte.Parse(P3.Text);
            IPAddress address = new IPAddress(p0, p1, p2, p3);
            string output = monitor.CheckIncoming(address);
            System.Threading.Thread.Sleep(3000);
            if (output.Contains("NO BOARD") || (output.Contains("ERROR") || output.Contains("FAILED")))
            {
                IncomingStatus.Text = output;
                IncomingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
            }
            else if (output.Contains("PASSED"))
            {
                IncomingStatus.Text = output;
                IncomingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
            }
            else if (output.Contains("BLOCKED"))
            {
                IncomingStatus.Text = output;
                IncomingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
            }
            EnableButons();
        }

        private void CheckOutgoin_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            byte p0 = Byte.Parse(P0.Text);
            byte p1 = Byte.Parse(P1.Text);
            byte p2 = Byte.Parse(P2.Text);
            byte p3 = Byte.Parse(P3.Text);
            IPAddress address = new IPAddress(p0, p1, p2, p3);
            string output = monitor.CheckOutgoing(address);
            System.Threading.Thread.Sleep(3000);
            if (output.Contains("NO BOARD"))
            {
                OutgoingStatus.Text = output;
                OutgoingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
                System.Diagnostics.Debug.WriteLine(OutgoingStatus.Text);
            }
            else if (output.Contains("ERROR") || output.Contains("FAILED"))
            {
                OutgoingStatus.Text = output;
                OutgoingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
            }
            else if (output.Contains("PASSED"))
            {
                
                OutgoingStatus.Text = output;
                OutgoingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
            }
            else if (output.Contains("BLOCKED"))
            {
                OutgoingStatus.Text = output;
                OutgoingStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                System.Diagnostics.Debug.WriteLine(OutgoingStatus.Text);
            }
            EnableButons();
        }

        private void P0_LostFocus(object sender, RoutedEventArgs e)
        {
            P0.BorderBrush = (IsP0Correct = Byte.TryParse(P0.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void P1_LostFocus(object sender, RoutedEventArgs e)
        {
            P1.BorderBrush = (IsP1Correct = Byte.TryParse(P1.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void P2_LostFocus(object sender, RoutedEventArgs e)
        {
            P2.BorderBrush = (IsP2Correct = Byte.TryParse(P2.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }

        private void P3_LostFocus(object sender, RoutedEventArgs e)
        {
            P3.BorderBrush = (IsP3Correct = Byte.TryParse(P3.Text, out var i)) ? NormalBrush : RedBrush;
            if (CanSendRequest)
                EnableButons();
            else
                DisableButtons();
        }
    }
}
