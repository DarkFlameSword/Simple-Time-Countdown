using System.Reflection;
using Microsoft.Win32;

namespace TimeCountdown.Setup;

internal static class InstallerContext
{
    public const string ProductName = "Simple Time Countdown";
    private const string LegacyProductName = "Time Countdown";
    private const string AppExecutableName = "TimeCountdown.exe";
    private const string InstallerExecutableName = "Simple Time Countdown Setup.exe";
    private static readonly string DefaultInstallRootPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Programs",
        LegacyProductName);
    private static string _installRoot = ResolveCurrentInstallRoot();

    public static string ProductVersion =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "1.3.0";

    public static string ProductDisplayVersion
    {
        get
        {
            var version = ProductVersion;
            var metadataSeparator = version.IndexOf('+');
            return metadataSeparator >= 0 ? version[..metadataSeparator] : version;
        }
    }

    public static string InstallRoot => _installRoot;

    public static string DefaultInstallRoot => DefaultInstallRootPath;

    public static string InstallerDirectory => Path.Combine(InstallRoot, "Installer");

    public static string AppExecutablePath => Path.Combine(InstallRoot, AppExecutableName);

    public static string InstallerExecutablePath => Path.Combine(InstallerDirectory, InstallerExecutableName);

    public static string AppShortcutIconPath => AppExecutablePath;

    public static string InstallerShortcutIconPath => InstallerExecutablePath;

    public static string LocalDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TimeCountdown");

    public static string StartMenuDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Microsoft",
        "Windows",
        "Start Menu",
        "Programs",
        ProductName);

    public static string LegacyStartMenuDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Microsoft",
        "Windows",
        "Start Menu",
        "Programs",
        LegacyProductName);

    public static string DesktopShortcutPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
        $"{ProductName}.lnk");

    public static string LegacyDesktopShortcutPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
        $"{LegacyProductName}.lnk");

    public static string UninstallRegistryPath => @"Software\Microsoft\Windows\CurrentVersion\Uninstall\TimeCountdown";

    public static bool IsInstalled => File.Exists(AppExecutablePath);

    public static IReadOnlyList<string> AllDesktopShortcutPaths =>
    [
        DesktopShortcutPath,
        LegacyDesktopShortcutPath
    ];

    public static IReadOnlyList<string> AllStartMenuDirectories =>
    [
        StartMenuDirectory,
        LegacyStartMenuDirectory
    ];

    public static void SetInstallRoot(string installRoot)
    {
        if (string.IsNullOrWhiteSpace(installRoot))
        {
            _installRoot = DefaultInstallRootPath;
            return;
        }

        _installRoot = Path.GetFullPath(Environment.ExpandEnvironmentVariables(installRoot.Trim()));
    }

    private static string ResolveCurrentInstallRoot()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(UninstallRegistryPath);
            var installLocation = key?.GetValue("InstallLocation") as string;
            if (!string.IsNullOrWhiteSpace(installLocation))
            {
                return Path.GetFullPath(installLocation);
            }
        }
        catch
        {
        }

        return DefaultInstallRootPath;
    }
}
