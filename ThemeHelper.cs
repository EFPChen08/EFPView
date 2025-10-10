using System;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EFPView;
public enum AppTheme
{
    System = 0,
    Light = 1,
    Dark = 2
}

public static class ThemeHelper
{
    private const string Key = "AppTheme";

    /// 初始化（应用启动时调用一次）
    public static void Initialize()
    {
        var theme = Load();
        Apply(theme, save: false);
    }

    /// 应用主题（可在设置页点击后调用）
    public static void Apply(AppTheme theme, bool save = true)
    {
        var root = Window.Current.Content as FrameworkElement;
        if (root == null) return;

        switch (theme)
        {
            case AppTheme.Light:
                root.RequestedTheme = ElementTheme.Light;
                Settings.Set("Theme", 2);
                break;
            case AppTheme.Dark:
                root.RequestedTheme = ElementTheme.Dark;
                Settings.Set("Theme", 1);
                break;
            default:
                // 跟随系统
                root.RequestedTheme = ElementTheme.Default;
                Settings.Set("Theme", 0);
                break;
        }

        if (save) Save(theme);

        UpdateTitleBar(theme);
    }

    public static AppTheme Load()
    {
        var settings = ApplicationData.Current.LocalSettings;
        if (settings.Values.TryGetValue(Key, out var value) && value is int i && Enum.IsDefined(typeof(AppTheme), i))
            return (AppTheme)i;
        return AppTheme.System;
    }

    private static void Save(AppTheme theme)
    {
        ApplicationData.Current.LocalSettings.Values[Key] = (int)theme;
    }

    public static void FullUpdateTitleBar(AppTheme theme)
    {
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        var tb = titleBar;

        bool dark = theme == AppTheme.Dark ||
                    (theme == AppTheme.System && IsSystemInDarkMode());


        if (dark)
        {
            tb.ButtonForegroundColor = Colors.White;
            tb.ButtonBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 255, 255, 255);
            tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 255, 255, 255); // 轻微提亮
            tb.ButtonHoverForegroundColor = Color.FromArgb(160, 255, 255, 255); // 轻微提亮
            tb.ButtonPressedForegroundColor = Color.FromArgb(160, 255, 255, 255); // 轻微提亮
            tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 255, 255, 255);
        }
        else
        {
            tb.ButtonForegroundColor = Colors.Black;
            tb.ButtonBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 0, 0, 0);       // 轻微压暗
            tb.ButtonHoverForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonPressedForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 0, 0, 0);
        }
    }

    public static void UpdateTitleBar(AppTheme theme)
    {
        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
        if (titleBar == null) return;

        if (theme != AppTheme.System) return;

        bool dark = IsSystemInDarkMode();
        var tb = titleBar;

        if (dark)
        {
            tb.ButtonForegroundColor = Colors.White;
            tb.ButtonBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 255, 255, 255);
            tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 255, 255, 255);
            tb.ButtonHoverForegroundColor = Color.FromArgb(160, 255, 255, 255);
            tb.ButtonPressedForegroundColor = Color.FromArgb(160, 255, 255, 255);
            tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 255, 255, 255);
        }
        else
        {
            tb.ButtonForegroundColor = Colors.Black;
            tb.ButtonBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveBackgroundColor = Colors.Transparent;
            tb.ButtonInactiveForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonHoverBackgroundColor = Color.FromArgb(24, 0, 0, 0);
            tb.ButtonHoverForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonPressedForegroundColor = Color.FromArgb(160, 0, 0, 0);
            tb.ButtonPressedBackgroundColor = Color.FromArgb(48, 0, 0, 0);
        }
    }

    private static bool IsSystemInDarkMode()
    {
        var ui = new UISettings();
        var bg = ui.GetColorValue(UIColorType.Background);
        int luminance = (299 * bg.R + 587 * bg.G + 114 * bg.B) / 1000;
        return luminance < 128;
    }
}
