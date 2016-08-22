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

namespace Spotify.Overlay.Controls
{
    /// <summary>
    /// Interaktionslogik für SpotifyPreviousButton.xaml
    /// </summary>
    public partial class SpotifyPreviousButton : UserControl
    {
        public SpotifyPreviousButton()
        {
            InitializeComponent();
        }

        public event EventHandler<MouseButtonEventArgs> Clicked;

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
