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
using Windows.UI;
using CodeManager;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirewallUI.Dialogs
{

    public class DataEnteredEventArgs : EventArgs
    {
        public IPAddress Address { get; private set; }

        public PacketRule Rule{ get; private set; }

        public DataEnteredEventArgs(IPAddress Address, PacketRule Rule)
        {
            this.Address = Address;
            this.Rule = Rule;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IPAddressRuleDialog : ContentDialog
    {

        private readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        private readonly SolidColorBrush NormalBrush = new SolidColorBrush(Color.FromArgb(255,99,99,99));

        public event EventHandler<DataEnteredEventArgs> DataEntered;


        private bool EnablePrimaryButton => IsP0Correct && IsP1Correct && IsP2Correct && IsP3Correct && IsAllowIncomingCorrect && IsAllowOutgoingCorrect;

        private bool IsP0Correct = false;
        private bool IsP1Correct = false;
        private bool IsP2Correct = false;
        private bool IsP3Correct = false;
        private bool IsAllowIncomingCorrect = false;
        private bool IsAllowOutgoingCorrect = false;

        public IPAddressRuleDialog(XamlRoot root)
        {
            this.XamlRoot = root;
            this.InitializeComponent();
            this.IsPrimaryButtonEnabled = EnablePrimaryButton;
            this.PrimaryButtonClick += IPAddressRuleDialog_PrimaryButtonClick;
            P0.LostFocus += (s, e) =>
            {
                P0.BorderBrush = (IsP0Correct = Byte.TryParse(P0.Text, out var i)) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
                
            };
            P1.LostFocus += (s, e) =>
            {
                P1.BorderBrush =(IsP1Correct = Byte.TryParse(P1.Text, out var i)) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
                
            };
            P2.LostFocus += (s, e) =>
            {
                P2.BorderBrush = (IsP2Correct =  Byte.TryParse(P2.Text, out var i)) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
            };
            P3.LostFocus += (s, e) =>
            {
                P3.BorderBrush = (IsP3Correct =  Byte.TryParse(P3.Text, out var i)) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
                
            };
            AllowIncoming.SelectionChanged += (s, e) =>
            {
                AllowIncoming.BorderBrush = (IsAllowIncomingCorrect = AllowIncoming.SelectedIndex == 0 || AllowIncoming.SelectedIndex == 1) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
                
            };
            AllowOutgoing.SelectionChanged += (s, e) =>
            {
                AllowOutgoing.BorderBrush =(IsAllowOutgoingCorrect = AllowOutgoing.SelectedIndex == 0 || AllowOutgoing.SelectedIndex == 1) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;  
            };
            AllowOutgoing.LostFocus += (s, e) =>
            {
                AllowOutgoing.BorderBrush = (IsAllowOutgoingCorrect = AllowOutgoing.SelectedIndex == 0 || AllowOutgoing.SelectedIndex == 1) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
            };

            AllowIncoming.LostFocus += (s, e) =>
            {
                AllowIncoming.BorderBrush = (IsAllowIncomingCorrect = AllowIncoming.SelectedIndex == 0 || AllowIncoming.SelectedIndex == 1) ? NormalBrush : RedBrush;
                this.IsPrimaryButtonEnabled = EnablePrimaryButton;
            };
        }
        private void IPAddressRuleDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            byte p0 = Byte.Parse(P0.Text);
            byte p1 = Byte.Parse(P1.Text);
            byte p2 = Byte.Parse(P2.Text);
            byte p3 = Byte.Parse(P3.Text);
            CodeManager.IPAddress address = new CodeManager.IPAddress(p0, p1, p2, p3);
            PacketRule rule = new PacketRule(AllowIncoming.SelectedIndex == 0, AllowOutgoing.SelectedIndex == 0);
            DataEntered?.Invoke(sender, new DataEnteredEventArgs(address, rule));
        }
    }
}
