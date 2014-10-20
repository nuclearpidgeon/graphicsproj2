using Project2.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Project2.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage
    {

        private Project2Game game;

        public GamePage()
        {
            this.InitializeComponent();

            Window.Current.Content = this;
            Window.Current.Activate();

            game = new Project2Game();
            game.PauseRequest += game_PauseRequest;
            if(!game.IsRunning) game.Run(this);
        }

        void game_PauseRequest(object sender, EventArgs e)
        {
            updatePauseBtn();
        }

        private void unpauseBtn_Click(object sender, RoutedEventArgs e)
        {
            game.togglePaused();
            updatePauseBtn();
        }

        private void menuBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Content = new MainPage();
            Window.Current.Activate();
        }

        private void restartBtn_Click(object sender, RoutedEventArgs e)
        {
            game.restartGame();
            if(game.isPaused()) game.togglePaused();
            updatePauseBtn();
        }

        private void pauseBar_Opened(object sender, object e)
        {
            updatePauseBtn();
        }
        private void updatePauseBtn()
        {
            bool paused = game.isPaused();
            if (paused)
            {
                pauseBar.IsSticky = pauseBar.IsOpen = true;
                unpauseBtn.Label = "Unpause";
                unpauseBtn.Icon = new SymbolIcon(Symbol.Play);
            }
            else
            {
                pauseBar.IsSticky = false;
                unpauseBtn.Label = "Pause";
                unpauseBtn.Icon = new SymbolIcon(Symbol.Pause);
            }

        }




    }
}
