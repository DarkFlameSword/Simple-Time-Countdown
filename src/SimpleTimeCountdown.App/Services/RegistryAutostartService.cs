using Microsoft.Win32;

namespace TimeCountdown.Services;

public sealed class RegistryAutostartService : IAutostartService
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string EntryName = "TimeCountdown";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: false);
        var value = key?.GetValue(EntryName) as string;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var currentPath = Quote(Environment.ProcessPath ?? string.Empty);
        return string.Equals(value, currentPath, StringComparison.OrdinalIgnoreCase);
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath, writable: true);
        if (key is null)
        {
            return;
        }

        if (enabled)
        {
            key.SetValue(EntryName, Quote(Environment.ProcessPath ?? string.Empty));
        }
        else
        {
            key.DeleteValue(EntryName, throwOnMissingValue: false);
        }
    }

    private static string Quote(string path) => $"\"{path}\"";
}

