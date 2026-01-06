using MLeaderTextSetup.ViewModels;
using System;
using System.Windows;

namespace MLeaderTextSetup.Views
{
    /// <summary>
    /// Interaction logic for TextSetupWindow.xaml
    /// </summary>
    public partial class TextSetupWindow : Window
    {
        public TextSetupWindow()
        {
            InitializeComponent();
            DataContext = new TextSetupViewModel(() => this.Close());
        }
    }
}
