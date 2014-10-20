using Project2.Common;
using Project2.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Project2
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        MainPage mainPage;
        Frame rootFrame;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
                // Currently we have no save state, stretch goal
            }

            InitMainPage();
        }

        public void InitMainPage()
        {
            mainPage = Window.Current.Content as MainPage;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (mainPage == null)
            {
                mainPage = new MainPage();

                // Retrieve the root Frame to act as the navigation context and navigate to the first page
                // Don't change the name of "rootFrame" in MainPage.xaml unless you change it here to match.
                rootFrame = (Frame)mainPage.FindName("rootFrame");

                // Associate the frame with a SuspensionManager key.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // Place the main page in the current Window.
                Window.Current.Content = mainPage;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MenuMainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when you wish to switch to another screen. The content of rootFrame is switched to
        /// a new instance of the supplied `nextScreen` argument.
        /// </summary>
        /// <param name="nextScreen">The Type of the page you wish to move to. typeof(Example)</param>
        public static void MoveToScreen(Type nextScreen)
        {
            MainPage mainPage = Window.Current.Content as MainPage;
            Frame rootFrame = (Frame)mainPage.FindName("rootFrame");
            rootFrame.Navigate(nextScreen);
        }
    }
}
