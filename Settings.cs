using Windows.Storage;

namespace EFPView;
public static class Settings
{
    private static readonly ApplicationDataContainer Local = ApplicationData.Current.LocalSettings;
    private const string Prefix = "EFPView."; // 防止键名冲突，可选

    // 写入string/int/bool/double 等原生类型
    public static void Set<T>(string key, T value)
    {
        Local.Values[Prefix + key] = value;
    }

    // 读取（带默认值）
    public static T Get<T>(string key, T defaultValue = default)
    {
        if (Local.Values.TryGetValue(Prefix + key, out var obj) && obj is T t)
            return t;
        return defaultValue;
    }

    // 删除
    public static void Remove(string key) => Local.Values.Remove(Prefix + key);
}
