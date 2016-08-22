using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Drawing;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;

namespace Spotify.Overlay
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool connectedToSpotify = false;
        private SpotifyLocalAPI api;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Auto Position Setting
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Config.EnableDoubleClickRealign)
            {
                this.AutoAlignPosition(Config.UseAnimationWhenRealign); 
            }
        }

        private void AutoAlignPosition(bool animate = false)
        {
            System.Windows.Size r = SystemParameters.WorkArea.Size;

            double targetTop = r.Height - this.Height;
            double targetLeft = r.Width - this.Width;

            if (!animate)
            {
                this.Top = targetTop;
                this.Left = targetLeft;
            }
            else
            {
                QuadraticEase easing = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                DoubleAnimation animatorTop = new DoubleAnimation(this.Top, targetTop, new Duration(TimeSpan.FromSeconds(0.4)), FillBehavior.Stop) { AutoReverse = false, EasingFunction = easing };
                DoubleAnimation animatorLeft = new DoubleAnimation(this.Left, targetLeft, new Duration(TimeSpan.FromSeconds(0.4)), FillBehavior.Stop) { AutoReverse = false, EasingFunction = easing };

                this.BeginAnimation(Window.LeftProperty, animatorLeft);
                this.BeginAnimation(Window.TopProperty, animatorTop);
            }
        }
        #endregion

        #region UI Updating
        private void SetTexts(string name = "", string artist = "")
        {
            this.Dispatcher.Invoke(new Action(() => {
                if (name != "") lblSongTitle.Content = name;
                if (artist != "") lblArtist.Content = artist;
            }));
        }
        private void ApplyTrack(Track t)
        {
            if(t.IsAd())
            {
                SetTexts("Advertisement", " ");
            }
            else
            {
                SetTexts(t.TrackResource.Name, t.ArtistResource.Name);
            }
        }
        private void SetOpacity(double to, bool animate = false)
        {
            this.Dispatcher.Invoke(new Action(() => {
                double from = this.Opacity;
                double target = to;

                if (animate)
                {
                    QuadraticEase easing = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
                    DoubleAnimation opacityAnimation = new DoubleAnimation(from, target, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.HoldEnd) { EasingFunction = easing, AutoReverse = false,  };

                    this.BeginAnimation(Window.OpacityProperty, opacityAnimation);
                }
                else
                {
                    this.Opacity = target;
                }
            }));
        }
        #endregion

        #region Spotify
        public bool ConnectedToSpotify
        {
            get
            {
                return connectedToSpotify;
            }
            set
            {
                if (value == false)
                {
                    playPauseButton.IsPlaying = false;
                }

                connectedToSpotify = value;
            }
        }

        private async Task CheckSpotify(bool autoStart = false)
        {
            SetTexts("Checking Spotify...", " ");

            bool tryConnect = false;
            bool webHelper = false, mainApp = false;
            ConnectedToSpotify = false;

            api = new SpotifyLocalAPI();

            await Task.Run(() => { webHelper = SpotifyLocalAPI.IsSpotifyWebHelperRunning(); if (webHelper) mainApp = SpotifyLocalAPI.IsSpotifyRunning(); });

            if (!autoStart)
            {
                if (!webHelper)
                {
                    SetTexts("Spotify Web Helper not running", "Not connected");
                }
                else
                {
                    if (!mainApp)
                    {
                        SetTexts("Spotify not running", "Not connected");
                    }
                    else
                    {
                        tryConnect = true;
                    }
                } 
            }
            else
            {
                if (!webHelper)
                {
                    SetTexts("Starting Web Helper...", " ");
                    await Task.Run(() => { SpotifyLocalAPI.RunSpotifyWebHelper(); }); 
                }

                if(!mainApp)
                {
                    SetTexts("Starting Spotify App...", " ");
                    await Task.Run(() => { SpotifyLocalAPI.RunSpotify(); });
                }

                tryConnect = true;
            }

            if(tryConnect)
            {
                SetTexts("Connecting to Spotify...", " ");
                bool connection = await Task.Run(() => { return api.Connect(); });

                if (!connection)
                {
                    SetTexts("Connection to Spotify failed", "Not connected");
                }
                else
                {
                    SetTexts("Successfully connected!", "Connected");

                    ConnectedToSpotify = true;

                    await StartService();
                }
            }
        }

        private async Task StartService()
        {
            api.OnPlayStateChange += Api_OnPlayStateChange;
            api.OnTrackChange += Api_OnTrackChange;

            api.ListenForEvents = true;

            StatusResponse s = await Task.Run<StatusResponse>(() => { return api.GetStatus(); });
            
            if(s != null)
            {
                ApplyTrack(s.Track);

                playPauseButton.IsPlaying = s.Playing;
                if (!s.Playing) if (Config.AutoLowerOpacityWhenPaused) SetOpacity(Config.NormalOpacity, Config.UseAnimationWhenLoweringOpacity);

                if(!s.Playing && s.PlayEnabled && Config.AutoConnectAtStart)
                {
                    api.Play();
                }
            }
        }

        private void Api_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            ApplyTrack(e.NewTrack);
        }

        private void Api_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            if (e.Playing)
            {
                playPauseButton.IsPlaying = true;
                if (Config.AutoLowerOpacityWhenPaused) SetOpacity(Config.NormalOpacity, Config.UseAnimationWhenLoweringOpacity);
            }
            else
            {
                playPauseButton.IsPlaying = false;
                if (Config.AutoLowerOpacityWhenPaused) SetOpacity(0.15, Config.UseAnimationWhenLoweringOpacity);
            }
        }
        #endregion

        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            this.AutoAlignPosition(Config.UseAnimationsAtStartup);

            this.SetOpacity(Config.NormalOpacity, Config.UseAnimationsAtStartup);

            if (Config.AutoConnectAtStart)
            {
                await this.CheckSpotify(Config.StartIfNotRunning);
            }
        }

        #region Interaction
        private async void playPauseButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (ConnectedToSpotify)
            {
                if (playPauseButton.IsPlaying)
                {
                    api.Pause();
                }
                else
                {
                    api.Play();
                }
            }
            else
            {
                await CheckSpotify(true);
            }
        }

        private void prevButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (ConnectedToSpotify)
            {
                api.Previous();
            }
        }

        private void nextButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (ConnectedToSpotify)
            {
                api.Skip();
            }
        }

        private void settingsButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            new Settings().ShowDialog();
        }

        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y < 10)
            {
                this.DragMove();
            }
        }
        #endregion
    }
}
