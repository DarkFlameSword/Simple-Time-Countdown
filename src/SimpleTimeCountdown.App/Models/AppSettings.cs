namespace TimeCountdown.Models;

public sealed class AppSettings
{
    public bool AlwaysOnTop { get; set; } = true;

    public bool LaunchAtStartup { get; set; }

    public bool HideOnCloseToTray { get; set; } = true;

    public bool DesktopLayerEnabled { get; set; }

    public double PanelOpacity { get; set; } = 0.96;

    public string SelectedFilter { get; set; } = "All";

    public int DefaultReminderMinutesBefore { get; set; } = 24 * 60;

    public string DefaultTimeZoneId { get; set; } = TimeZoneInfo.Local.Id;

    public string LanguageCode { get; set; } = "en";

    public double WindowLeft { get; set; } = double.NaN;

    public double WindowTop { get; set; } = double.NaN;

    public double WindowWidth { get; set; } = 420;

    public double WindowHeight { get; set; } = 760;
}
