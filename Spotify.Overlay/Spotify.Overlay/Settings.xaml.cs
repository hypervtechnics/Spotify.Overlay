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
using System.Windows.Shapes;

namespace Spotify.Overlay
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chkAutoConnectAtStart.IsChecked = Config.AutoConnectAtStart;
            chkAutoLowerOpacityWhenPaused.IsChecked = Config.AutoLowerOpacityWhenPaused;
            chkAutoStartWhenConnecting.IsChecked = Config.StartIfNotRunning;
            chkEnableDoubleClickRealign.IsChecked = Config.EnableDoubleClickRealign;
            chkUseAnimationsAtStartup.IsChecked = Config.UseAnimationsAtStartup;
            chkUseAnimationWhenLoweringOpacity.IsChecked = Config.UseAnimationWhenLoweringOpacity;
            chkUseAnimationWhenRealign.IsChecked = Config.UseAnimationWhenRealign;
            slNormalOpacity.Value = Config.NormalOpacity;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Config.AutoConnectAtStart = chkAutoConnectAtStart.IsChecked.Value;
            Config.AutoLowerOpacityWhenPaused = chkAutoLowerOpacityWhenPaused.IsChecked.Value;
            Config.EnableDoubleClickRealign = chkEnableDoubleClickRealign.IsChecked.Value;
            Config.NormalOpacity = slNormalOpacity.Value;
            Config.StartIfNotRunning = chkAutoStartWhenConnecting.IsChecked.Value;
            Config.UseAnimationsAtStartup = chkUseAnimationsAtStartup.IsChecked.Value;
            Config.UseAnimationWhenLoweringOpacity = chkUseAnimationWhenLoweringOpacity.IsChecked.Value;
            Config.UseAnimationWhenRealign = chkUseAnimationWhenRealign.IsChecked.Value;

            Config.SaveConfig();

            this.Close();
        }
    }
}
