using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Microsoft.Win32;

namespace TimeCountdown.Setup;

internal static class InstallerEngine
{
    public static void Install(InstallOptions options, IProgress<InstallerProgress>? progress)
    {
        StopRunningApp();

        var tempDir = Path.Combine(Path.GetTempPath(), $"SimpleTimeCountdownSetup_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            Report(progress, 4, "准备安装", "正在检查安装包内容。");
            ExtractPayloadZip(tempDir);

            Report(progress, 12, "准备文件", "正在创建应用目录。");
            PrepareInstallRoot();

            Report(progress, 20, "复制应用文件", "正在写入程序文件。");
            CopyDirectory(tempDir, InstallerContext.InstallRoot, progress);

            Report(progress, 82, "完成安装配置", "正在注册快捷方式和卸载信息。");
            InstallBootstrapperCopy();
            CreateShortcuts();
            RegisterUninstall();

            if (options.LaunchAfterInstall)
            {
                Report(progress, 94, "启动应用", $"正在打开 {InstallerContext.ProductName}。");
                Process.Start(new ProcessStartInfo
                {
                    FileName = InstallerContext.AppExecutablePath,
                    UseShellExecute = true,
                    WorkingDirectory = InstallerContext.InstallRoot
                });
            }

            Report(progress, 100, "安装完成", $"{InstallerContext.ProductName} 已可以开始使用。");
        }
        finally
        {
            TryDeleteDirectory(tempDir);
        }
    }

    public static void Uninstall(InstallOptions options, IProgress<InstallerProgress>? progress)
    {
        Report(progress, 8, "准备卸载", "正在关闭运行中的应用。");
        StopRunningApp();

        Report(progress, 28, "移除快捷方式", "正在清理桌面、开始菜单和卸载信息。");
        RemoveShortcutsAndRegistry();

        if (Directory.Exists(InstallerContext.InstallRoot))
        {
            Report(progress, 58, "删除软件文件", "正在移除已安装的程序文件。");
            LaunchDeferredCleanup();
        }

        if (options.RemoveLocalData)
        {
            Report(progress, 82, "删除本地数据", "正在删除倒计时和设置数据。");
            TryDeleteDirectory(InstallerContext.LocalDataDirectory);
        }

        Report(progress, 100, "卸载完成", $"{InstallerContext.ProductName} 已移除。");
    }

    private static void ExtractPayloadZip(string destinationDirectory)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("TimeCountdown-portable.zip", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            throw new InvalidOperationException("Installer payload is missing.");
        }

        var zipPath = Path.Combine(destinationDirectory, "TimeCountdown-portable.zip");
        using var stream = assembly.GetManifestResourceStream(resourceName) ??
                           throw new InvalidOperationException("Unable to open installer payload.");
        using (var file = File.Create(zipPath))
        {
            stream.CopyTo(file);
        }

        ZipFile.ExtractToDirectory(zipPath, destinationDirectory, overwriteFiles: true);
        File.Delete(zipPath);
    }

    private static void PrepareInstallRoot()
    {
        Directory.CreateDirectory(InstallerContext.InstallRoot);
        foreach (var path in Directory.EnumerateFileSystemEntries(InstallerContext.InstallRoot))
        {
            TryDeletePath(path);
        }
    }

    private static void InstallBootstrapperCopy()
    {
        Directory.CreateDirectory(InstallerContext.InstallerDirectory);

        var currentExe = Environment.ProcessPath ??
                         throw new InvalidOperationException("Unable to locate the setup executable.");
        File.Copy(currentExe, InstallerContext.InstallerExecutablePath, overwrite: true);
    }

    private static void CreateShortcuts()
    {
        RemoveShortcutArtifacts();
        Directory.CreateDirectory(InstallerContext.StartMenuDirectory);

        CreateShortcut(
            InstallerContext.DesktopShortcutPath,
            InstallerContext.AppExecutablePath,
            InstallerContext.InstallRoot,
            InstallerContext.AppShortcutIconPath,
            null);

        CreateShortcut(
            Path.Combine(InstallerContext.StartMenuDirectory, $"{InstallerContext.ProductName}.lnk"),
            InstallerContext.AppExecutablePath,
            InstallerContext.InstallRoot,
            InstallerContext.AppShortcutIconPath,
            null);

        CreateShortcut(
            Path.Combine(InstallerContext.StartMenuDirectory, $"Uninstall {InstallerContext.ProductName}.lnk"),
            InstallerContext.InstallerExecutablePath,
            InstallerContext.InstallerDirectory,
            InstallerContext.InstallerShortcutIconPath,
            "--uninstall");
    }

    private static void RegisterUninstall()
    {
        using var key = Registry.CurrentUser.CreateSubKey(InstallerContext.UninstallRegistryPath);

        key?.SetValue("DisplayName", InstallerContext.ProductName);
        key?.SetValue("Publisher", InstallerContext.ProductName);
        key?.SetValue("DisplayVersion", InstallerContext.ProductVersion);
        key?.SetValue("InstallLocation", InstallerContext.InstallRoot);
        key?.SetValue("DisplayIcon", InstallerContext.AppExecutablePath);
        key?.SetValue("UninstallString", $"\"{InstallerContext.InstallerExecutablePath}\" --uninstall");
        key?.SetValue("NoModify", 1, RegistryValueKind.DWord);
        key?.SetValue("NoRepair", 1, RegistryValueKind.DWord);
    }

    private static void RemoveShortcutsAndRegistry()
    {
        RemoveShortcutArtifacts();
        Registry.CurrentUser.DeleteSubKeyTree(InstallerContext.UninstallRegistryPath, throwOnMissingSubKey: false);
    }

    private static void RemoveShortcutArtifacts()
    {
        foreach (var shortcutPath in InstallerContext.AllDesktopShortcutPaths)
        {
            TryDeletePath(shortcutPath);
        }

        foreach (var startMenuDirectory in InstallerContext.AllStartMenuDirectories)
        {
            TryDeleteDirectory(startMenuDirectory);
        }
    }

    private static void LaunchDeferredCleanup()
    {
        var cleanupScript = Path.Combine(Path.GetTempPath(), $"SimpleTimeCountdownCleanup_{Guid.NewGuid():N}.cmd");
        var content = $"""
                       @echo off
                       ping 127.0.0.1 -n 3 >nul
                       rmdir /s /q "{InstallerContext.InstallRoot}"
                       del /q "%~f0"
                       """;
        File.WriteAllText(cleanupScript, content);

        Process.Start(new ProcessStartInfo
        {
            FileName = cleanupScript,
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

    private static void StopRunningApp()
    {
        foreach (var process in Process.GetProcessesByName("TimeCountdown"))
        {
            try
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }
            catch
            {
            }
        }
    }

    private static void CopyDirectory(string source, string destination, IProgress<InstallerProgress>? progress)
    {
        Directory.CreateDirectory(destination);

        foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(source, directory);
            Directory.CreateDirectory(Path.Combine(destination, relative));
        }

        var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        for (var index = 0; index < files.Length; index++)
        {
            var file = files[index];
            var relative = Path.GetRelativePath(source, file);
            var target = Path.Combine(destination, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite: true);

            var percent = 20 + (int)Math.Round(((index + 1d) / files.Length) * 58);
            Report(progress, percent, "复制应用文件", $"正在安装 {relative}");
        }
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string workingDirectory, string iconPath, string? arguments)
    {
        var shellType = Type.GetTypeFromProgID("WScript.Shell") ??
                        throw new InvalidOperationException("WScript.Shell is unavailable.");
        dynamic shell = Activator.CreateInstance(shellType)!;
        dynamic shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = workingDirectory;
        shortcut.IconLocation = iconPath;
        if (!string.IsNullOrWhiteSpace(arguments))
        {
            shortcut.Arguments = arguments;
        }

        shortcut.Save();
    }

    private static void Report(IProgress<InstallerProgress>? progress, int percent, string title, string detail)
    {
        progress?.Report(new InstallerProgress(percent, title, detail));
    }

    private static void TryDeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static void TryDeletePath(string path)
    {
        if (Directory.Exists(path))
        {
            TryDeleteDirectory(path);
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
