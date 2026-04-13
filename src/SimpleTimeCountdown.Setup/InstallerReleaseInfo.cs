namespace TimeCountdown.Setup;

internal static class InstallerReleaseInfo
{
    public static IReadOnlyList<string> Highlights { get; } =
    [
        "品牌名更新为 Simple Time Countdown，并统一了安装器、托盘和打包产物命名。",
        "主窗口改为更轻量的图标化操作按钮，卡片操作区也统一为符号按钮。",
        "补齐了设置项体验，支持中英文切换、桌面层模式和默认时区显示。",
        "重新设计了应用图标，并同步应用到主程序、安装器和 MSIX 资源。"
    ];

    public static string BuildHighlightsText()
    {
        return string.Join(Environment.NewLine, Highlights.Select(item => $"- {item}"));
    }
}
