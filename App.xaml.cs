using EFPView;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EFPView
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
    /// </summary>
    public sealed partial class App : Application
    {
        UISettings _uiSettings;
        CoreDispatcher _uiDispatcher;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        bool IsSystemDark()
        {
            var bg = _uiSettings.GetColorValue(UIColorType.Background);
            return bg == Colors.Black;
        }

        void ApplyThemeAwareCaptionButtons()
        {
            bool dark = IsSystemDark();
            var tb = ApplicationView.GetForCurrentView().TitleBar;

            tb.ButtonBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (dark)
            {
                tb.ButtonForegroundColor = Colors.White;
                tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 255, 255, 255);
                tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 255, 255, 255); // 轻微提亮
                tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 255, 255, 255);
            }
            else
            {
                tb.ButtonForegroundColor = Colors.Black;
                tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 0, 0, 0);
                tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 0, 0, 0);       // 轻微压暗
                tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 0, 0, 0);
            }
        }

        async void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            var dispatcher = _uiDispatcher ?? CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (dispatcher == null) return;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ApplyThemeAwareCaptionButtons();
            });
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }


            _uiDispatcher = Window.Current.Dispatcher;

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page, configuring
                    // the new page by passing required information as a navigation parameter.
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }

            _uiSettings = new UISettings();
            ApplyThemeAwareCaptionButtons();

            _uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load page '{e.SourcePageType.FullName}'.");
        }

        /// <summary>
        /// Invoked when application execution is being suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            if (_uiSettings != null)
            {
                _uiSettings.ColorValuesChanged -= UiSettings_ColorValuesChanged;
            }

            // TODO: Save application state and stop any background activity

            deferral.Complete();
        }
    }
}
