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
    /// Interaktionslogik für SpotifyPlayPauseButton.xaml
    /// </summary>
    public partial class SpotifyPlayPauseButton : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> Clicked;
        private bool isPaused;

        public SpotifyPlayPauseButton()
        {
            InitializeComponent();

            IsPlaying = true;
        }

        public bool IsPlaying
        {
            get
            {
                return isPaused;
            }
            set
            {

                if(isPaused != value)
                {
                    isPaused = value;

                    if (isPaused)
                    {
                        this.Dispatcher.Invoke(new Action(() => buttonContent.Content = new SpotifyPauseBase()));
                    }
                    else
                    {
                        this.Dispatcher.Invoke(new Action(() => buttonContent.Content = new SpotifyPlayBase()));
                    }
                }
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Clicked != null) Clicked(this, e);
        }
    }
}
