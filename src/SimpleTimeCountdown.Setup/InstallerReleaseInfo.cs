namespace TimeCountdown.Setup;

internal static class InstallerReleaseInfo
{
    public static IReadOnlyList<string> Highlights { get; } =
    [
        "主界面交互简化：移除过滤下拉，仅保留搜索输入，提高录入与浏览效率。",
        "卡片列表滚动优化：右侧滚动条改为像素级平滑滚动，不再按卡片高度跳跃。",
        "设置项收敛：已到期阈值固定为 0，避免误配导致状态判断不一致。",
        "安装流程优化：安装路径输入框改为只读，点击“更改”后通过文件夹选择器设置目录。"
    ];

    public static string BuildHighlightsText()
    {
        return string.Join(Environment.NewLine, Highlights.Select(item => $"- {item}"));
    }
}
