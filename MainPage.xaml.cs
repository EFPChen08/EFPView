using EFPView.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EFPView
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Loaded += (_, __) => Window.Current.SetTitleBar(TitleBarDragRegion);

            ContentFrame.Navigated += ContentFrame_Navigated;
            Loaded += MainPage_Loaded;

            NavView.DisplayModeChanged += (_, __) => UpdateTitleBarLayout();
            NavView.Loaded += (s, e) =>
                _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateTitleBarLayout);
            NavView.SizeChanged += (s, e) => UpdateTitleBarLayout();

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                if (ContentFrame.CanGoBack) { e.Handled = true; ContentFrame.GoBack(); }
            };
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.Content == null) ContentFrame.Navigate(typeof(HomePage));
            UpdateBackUI();

            var theme = Settings.Get("Theme", 0);
            var nava = Settings.Get("NavViewMode", 0);

            if (theme == 0)
            {
                ThemeHelper.Apply(AppTheme.System);
            }
            else if (theme == 1)
            {
                ThemeHelper.Apply(AppTheme.Dark);
            }
            else if (theme == 2)
            {
                ThemeHelper.Apply(AppTheme.Light);
            }

            if (nava == 0)
            {
                App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
            }
            else if (nava == 1)
            {
                App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
            }
            else if (nava == 2)
            {
                App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
            }
        }

        private void UpdateTitleBarLayout()
        {
            var isTop = NavView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
            var displayMode = NavView.DisplayMode;

            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            if (isTop)
            {
                TopTitleBarRow.Height = new GridLength(32);
                TopTitleBar.Visibility = Visibility.Visible;
                AppTitleBar.Visibility = Visibility.Collapsed;
                HamburgerPlaceholderCol.Width = new GridLength(0);
                Window.Current.SetTitleBar(TopTitleBar);
                TitleBarBackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                TopTitleBarRow.Height = new GridLength(0);
                TopTitleBar.Visibility = Visibility.Collapsed;
                AppTitleBar.Visibility = Visibility.Visible;
                bool needHamburger = (displayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal);
                HamburgerPlaceholderCol.Width = new GridLength(needHamburger ? 48 : 0);
                Window.Current.SetTitleBar(TitleBarDragRegion);
            }
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            UpdateBackUI();
            UpdateNavViewSelection(e.SourcePageType);
        }

        private void UpdateBackUI()
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

        }

        private void UpdateNavViewSelection(Type pageType)
        {
            if (pageType == typeof(SettingsPage))
            {
                NavView.SelectedItem = NavView.SettingsItem;
                return;
            }

            foreach (var item in NavView.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem nvi)
                {
                    var tag = nvi.Tag as string;
                    if ((tag == "home" && pageType == typeof(HomePage)) ||
                        (tag == "controls" && pageType == typeof(AboutPage)))
                    {
                        NavView.SelectedItem = nvi;
                        break;
                    }
                }
            }
        }


        private void TitleBarBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                if (ContentFrame.CurrentSourcePageType != typeof(SettingsPage))
                    ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.InvokedItemContainer is Microsoft.UI.Xaml.Controls.NavigationViewItem nvi)
            {
                switch (nvi.Tag as string)
                {
                    case "home":
                        if (ContentFrame.CurrentSourcePageType != typeof(HomePage))
                            ContentFrame.Navigate(typeof(HomePage));
                        break;
                    case "controls":
                        if (ContentFrame.CurrentSourcePageType != typeof(AboutPage))
                            ContentFrame.Navigate(typeof(AboutPage));
                        break;
                }
            }
        }

    }
}
