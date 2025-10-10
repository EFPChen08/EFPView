using EFPView;
using EFPView.Pages;
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
using Windows.Foundation;

namespace EFPView
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
    /// </summary>
    /// 
    public sealed partial class App : Application
    {
        public static new App Current => (App)Application.Current;

        //public MainPage MainPage =>
        //    (Window.Current.Content as Frame)?.Content as MainPage;
        
        public static MainPage MainPage { get; private set; }
        public static SettingsPage SettingsPage { get; private set; }

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

        public static bool IsAppDarkTheme()
        {
            if (Window.Current?.Content is FrameworkElement root)
            {
                return root.ActualTheme == ElementTheme.Dark;
            }
            return false; // 没拿到根元素时给个默认
        }

        void ApplyThemeAwareCaptionButtons()
        {
            ThemeHelper.UpdateTitleBar(IsAppDarkTheme() ? AppTheme.Dark : AppTheme.Light);
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
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 800);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame = new Frame();
                Window.Current.Content = frame;
            }

            frame.Navigated += Frame_Navigated;
            if (frame.Content == null)
                frame.Navigate(typeof(MainPage), e.Arguments);

            Window.Current.Activate();

            _uiSettings = new UISettings();

            ApplyThemeAwareCaptionButtons();
            UpdateCaptionButtonsByAppTheme();

            if (Window.Current.Content is FrameworkElement root)
            {
                root.ActualThemeChanged += (_, __) => UpdateCaptionButtonsByAppTheme();
            }

            _uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        void UpdateCaptionButtonsByAppTheme()
        {
            ThemeHelper.FullUpdateTitleBar(IsAppDarkTheme() ? AppTheme.Dark : AppTheme.Light);
        }


        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(MainPage))
            {
                MainPage = ((Frame)sender).Content as MainPage;
                // ((Frame)sender).Navigated -= Frame_Navigated;
            }
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
