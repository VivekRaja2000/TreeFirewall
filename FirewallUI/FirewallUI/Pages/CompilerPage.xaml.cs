using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CodeManager;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirewallUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompilerPage : Page
    {
        private List<IPAddress> addresses = new List<IPAddress>();
        private List<PacketRule> rules = new List<PacketRule>();

        private SolidColorBrush RedBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
        private readonly SolidColorBrush NormalBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 99, 99, 99));

        private bool IsWifiNameAvailable => WiFiName.Text.Length > 0;
        private bool IsPasswordAvailable => Password.Password.Length >= 8;
        private bool EnableUpload => addresses.Count > 0 && IsWifiNameAvailable && IsPasswordAvailable;

        private ObservableCollection<RuleItem> displayItems = new ObservableCollection<RuleItem>();

        private ObservableCollection<TextBlock> OutputBlocks = new ObservableCollection<TextBlock>();
        public CompilerPage()
        {
            this.InitializeComponent();
            RulesScroller.Focus(FocusState.Pointer);
            CompilationOutput.ItemsSource = OutputBlocks;
            Upload.IsEnabled = EnableUpload;
            WiFiName.LostFocus += (s, e) =>
            {
                WiFiName.BorderBrush = WiFiName.Text.Length == 0 ? RedBrush : NormalBrush;
                Upload.IsEnabled = EnableUpload;
            };
            Password.LostFocus += (s, e) =>
            {
                Password.BorderBrush = Password.Password.Length < 8 ? RedBrush : NormalBrush;
                Upload.IsEnabled = EnableUpload;
            };
        }

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.IPAddressRuleDialog dialog = new Dialogs.IPAddressRuleDialog(this.XamlRoot);
            dialog.DataEntered += Dialog_DataEntered;
            await dialog.ShowAsync();
        }

        private void Dialog_DataEntered(object sender, Dialogs.DataEnteredEventArgs e)
        {
            addresses.Add(e.Address);
            rules.Add(e.Rule);
            RuleItem item = new RuleItem(e.Address, e.Rule);
            item.OnDeleteRequested += Item_OnDeleteRequested;
            RulesList.Children.Add(item);
            Upload.IsEnabled = EnableUpload;
        }

        private void Item_OnDeleteRequested(object sender, Guid e)
        {
            int position = RulesList.Children.IndexOf((RuleItem)sender);
            addresses.RemoveAt(position);
            rules.RemoveAt(position);
            RulesList.Children.RemoveAt(position);
            Upload.IsEnabled = EnableUpload;
        }

        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            
            InputPanel.Visibility = Visibility.Collapsed;
            AddItem.Visibility = Visibility.Collapsed;
            RulesList.Visibility = Visibility.Collapsed;
            CompilationOutput.Visibility = Visibility.Visible;
            Upload.IsEnabled = false;
            CodeCompiler compiler = new CodeCompiler();
            Progress<string> progress = new Progress<string>(ChangeText);
            var result = await compiler.CompileAndUploadCode(addresses, rules, WiFiName.Text, Password.Password, progress);
            Upload.Visibility = Visibility.Collapsed;
            Upload.IsEnabled = EnableUpload;
            Done.Content = result ? "Upload Successful. Click to close" : "Upload Fail. Click to re-upload";
            Done.Visibility = Visibility.Visible;
        }

        private void ChangeText(string value)
        {
            if (value.StartsWith("stderr"))
            {
                value = value.Replace("stderr : ", "");
                value = value.Replace(Environment.NewLine, "");
                if (!String.IsNullOrWhiteSpace(value) || !String.IsNullOrEmpty(value))
                {
                    TextBlock block = new TextBlock();
                    block.Text = value;
                    block.Margin = new Thickness(0);
                    block.Foreground = RedBrush;
                    OutputBlocks.Add(block);
                }
            }
            else if (value.StartsWith("stdout"))
            {
                value = value.Replace("stdout : ", "");
                value = value.Replace(Environment.NewLine, "");
                if (!String.IsNullOrWhiteSpace(value) || !String.IsNullOrEmpty(value))
                {
                    TextBlock block = new TextBlock();
                    block.Text = value;
                    block.Margin = new Thickness(0);
                    OutputBlocks.Add(block);
                }
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            InputPanel.Visibility = Visibility.Visible;
            AddItem.Visibility = Visibility.Visible;
            RulesList.Visibility = Visibility.Visible;
            CompilationOutput.Visibility = Visibility.Collapsed;
            OutputBlocks.Clear();
            Upload.Visibility = Visibility.Visible;
            Done.Visibility = Visibility.Collapsed;
        }
    }
}
