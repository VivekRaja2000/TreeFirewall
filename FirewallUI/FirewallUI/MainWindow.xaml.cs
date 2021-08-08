using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FirewallUI
{
    
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly Pages.CompilerPage compilerPage = new Pages.CompilerPage();

        private readonly Pages.OutputPage outputPage = new Pages.OutputPage();

        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.IsNavigationStackEnabled = false;
        }

        private void Compiler_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = compilerPage;
        }

        private void ÖutputWindow_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = outputPage;
        }
    }
}
