using Project2.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Project2.GameSystems;

namespace Project2
{

    
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public OptionsPage()
        {
            this.InitializeComponent();

            // Setup the navigation helper
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += Window_SizeChanged;

            initView();
        }

        void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation())
            {
                this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a bindable group to Me.DefaultViewModel("Group")
            // TODO: Assign a collection of bindable items to Me.DefaultViewModel("Items")

            if (e.PageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (e.PageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    // TODO: Invoke Me.itemsViewSource.View.MoveCurrentTo() with the selected
                    //       item as specified by the value of pageState("SelectedItem")

                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (this.itemsViewSource.View != null)
            {
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState("SelectedItem")

            }
        }

        #region Logical page navigation

        // The split page isdesigned so that when the Window does have enough space to show
        // both the list and the dteails, only one pane will be shown at at time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <returns>True if the window should show act as one logical page, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Invoked with the Window changes size
        /// </summary>
        /// <param name="sender">The current Window</param>
        /// <param name="e">Event data that describes the new size of the Window</param>
        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
        }

        private bool CanGoBack()
        {
           return this.navigationHelper.CanGoBack();
        }
        private void GoBack()
        {
           this.navigationHelper.GoBack();
        }

        #endregion

        private void initView()
        {
            accelSensitiveSlider.Value = PersistentStateManager.accelSensitivity;
            accelSensitiveSlider.ValueChanged += AccelerometerSensitiveChanged;

            accelPhysAccuracySlider.Value = PersistentStateManager.physicsAccuracy;
            accelPhysAccuracySlider.ValueChanged += PhysicsAccuracyChanged;

            cboxMultithreadingPhysics.IsChecked = PersistentStateManager.physicsMultithreading;
            cboxMultithreadingPhysics.Checked += cboxMultithreadingPhysicsChecked;
            cboxMultithreadingPhysics.Unchecked += cboxMultithreadingPhysicsChecked;

            cboxDebugRender.IsChecked = PersistentStateManager.debugRender;
            cboxDebugRender.Checked += cboxDebugRenderChecked;
            cboxDebugRender.Unchecked += cboxDebugRenderChecked;

            cboxDynamicTimestep.IsChecked = PersistentStateManager.dynamicTimestep;
            cboxDynamicTimestep.Checked += cboxDynamicTimestepChecked;
            cboxDynamicTimestep.Unchecked += cboxDynamicTimestepChecked;

            UpdateButtons();
        }

        private void cboxMultithreadingPhysicsChecked(object sender, RoutedEventArgs e)
        {
            PersistentStateManager.physicsMultithreading = cboxMultithreadingPhysics.IsChecked.Value;
        }


        private void cboxDebugRenderChecked(object sender, RoutedEventArgs e)
        {
            PersistentStateManager.debugRender = cboxDebugRender.IsChecked.Value;
        }


        private void cboxDynamicTimestepChecked(object sender, RoutedEventArgs e)
        {
            PersistentStateManager.dynamicTimestep = cboxDynamicTimestep.IsChecked.Value;
        }

        private void TextureHighClick(object sender, RoutedEventArgs e)
        {
            SetTextureQuality(Quality.High);
        }
        private void TextureMediumClick(object sender, RoutedEventArgs e)
        {
            SetTextureQuality(Quality.Medium);
        }
        private void TextureLowClick(object sender, RoutedEventArgs e)
        {
            SetTextureQuality(Quality.Low);
        }

        private void SetTextureQuality(Quality q)
        {
            PersistentStateManager.textureQuality = q;
            UpdateButtons();
        }
        private void UpdateButtons()
        {
            Dictionary<Quality, Button> qualityDict = new Dictionary<Quality, Button>() { { Quality.Low, txture_low_btn }, { Quality.Medium, txture_med_btn }, { Quality.High, txture_high_btn } };
            foreach (KeyValuePair<Quality, Button> entry in qualityDict) entry.Value.IsEnabled = true;
            qualityDict[PersistentStateManager.textureQuality].IsEnabled = false;

            Dictionary<Quality, Button> lightingDict = new Dictionary<Quality, Button>() { { Quality.Low, lighting_low_btn }, { Quality.Medium, lighting_med_btn }, { Quality.High, lighting_high_btn } };
            foreach (KeyValuePair<Quality, Button> entry in lightingDict) entry.Value.IsEnabled = true;
            lightingDict[PersistentStateManager.lightingQuality].IsEnabled = false;
        }

        private void LightingHighClick(object sender, RoutedEventArgs e)
        {
            SetLightingQuality(Quality.High);
        }
        private void LightingMediumClick(object sender, RoutedEventArgs e)
        {
            SetLightingQuality(Quality.Medium);
        }
        private void LightingLowClick(object sender, RoutedEventArgs e)
        {
            SetLightingQuality(Quality.Low);
        }

        private void SetLightingQuality(Quality q)
        {
            PersistentStateManager.lightingQuality = q;
            UpdateButtons();
        }
        
        private void AccelerometerSensitiveChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            PersistentStateManager.accelSensitivity = e.NewValue;
        }
        private void PhysicsAccuracyChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int newVal = (int)e.NewValue;
            if (newVal == 0) newVal = 1;
            ((Slider)sender).Value = newVal;
            PersistentStateManager.physicsAccuracy = newVal;
        }
    }
}
