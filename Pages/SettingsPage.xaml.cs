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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EFPView.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            var theme = Settings.Get("Theme", 0);
            var nava = Settings.Get("NavViewMode", 0);
            var au = Settings.Get("Audio", 0);

            if (theme == 0)
            {
                ThemeComboBox.SelectedIndex = 0;
            }
            else if (theme == 1)
            {
                ThemeComboBox.SelectedIndex = 2;
            }
            else if (theme == 2)
            {
                ThemeComboBox.SelectedIndex = 1;
            }

            if (nava == 0)
            {
                BuJu.SelectedIndex = 0;
            }
            else if (nava == 1)
            {
                BuJu.SelectedIndex = 1;
            }
            else if (nava == 2)
            {
                BuJu.SelectedIndex = 2;
            }

            if (au == 1)
            {
                AudioToggle.IsOn = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                AudioToggle.IsOn = false;
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
            }
        }

        private void Auto_Checked(object sender, RoutedEventArgs e)
        {
            App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
            Settings.Set("NavViewMode", 0);
        }

        private void Left_Checked(object sender, RoutedEventArgs e)
        {
            App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
            Settings.Set("NavViewMode", 1);
        }

        private void Top_Checked(object sender, RoutedEventArgs e)
        {
            App.MainPage.NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
            Settings.Set("NavViewMode", 2);
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedIndex == 1) {
                ThemeHelper.Apply(AppTheme.Light);
            } else if (ThemeComboBox.SelectedIndex == 2)
            {
                ThemeHelper.Apply(AppTheme.Dark);
            } else {
                ThemeHelper.Apply(AppTheme.System);
            }
                
        }

        private void AudioToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var ts = (ToggleSwitch)sender;
            if (ts.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                Settings.Set("Audio", 1);
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                Settings.Set("Audio", 0);
            }
        }
    }
}
