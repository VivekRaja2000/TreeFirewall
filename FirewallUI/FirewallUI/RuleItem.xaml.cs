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
using CodeManager;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI;
using System.Numerics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirewallUI
{
    public sealed partial class RuleItem : UserControl
    {
        private readonly IPAddress Address;

        private readonly PacketRule packetRule;

        public event EventHandler<Guid> OnDeleteRequested;

        public readonly Guid ItemId;

        public RuleItem(IPAddress Address, PacketRule packetRule)
        {
            this.InitializeComponent();
            ItemId = Guid.NewGuid();
            this.Address = Address;
            this.packetRule = packetRule;
            this.IpAddress.Text = Address.ToIPString();
            this.AllowIncoming.Text = (packetRule.AllowIncoming) ? "Allow Incoming" : "Block Incoming";
            this.AllowOutgoing.Text = (packetRule.AllowIncoming) ? "Allow Outgoing" : "Block Outgoing";
            this.BorderThickness = new Thickness(5);
            this.BorderBrush = new SolidColorBrush(Colors.DarkGray);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            OnDeleteRequested?.Invoke(this, this.ItemId);
        }
    }
}
