using TimeCountdown.Models;
using TimeCountdown.Services;
using MediaBrush = System.Windows.Media.Brush;
using MediaBrushConverter = System.Windows.Media.BrushConverter;
using MediaBrushes = System.Windows.Media.Brushes;
using MediaSolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace TimeCountdown.ViewModels;

public sealed class CountdownItemViewModel : ObservableObject
{
    private readonly LocalizationService _localization = LocalizationService.Instance;
    private readonly CountdownItem _model;
    private string _remainingPrimary = string.Empty;
    private string _remainingSecondary = string.Empty;
    private string _deadlineDisplay = string.Empty;
    private string _statusText = string.Empty;
    private MediaBrush _statusForeground = MediaBrushes.White;
    private MediaBrush _statusBackground = MediaBrushes.Green;
    private MediaBrush _progressBrush = MediaBrushes.Green;
    private double _progressPercent;
    private bool _isOverdue;
    private bool _isUrgent;
    private bool _isToday;
    private bool _isSoon;
    private bool _isSafe;
    private CountdownThresholds _thresholds = CountdownThresholds.Default;

    public CountdownItemViewModel(CountdownItem model)
    {
        _model = model;
        _localization.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is "Item[]" or nameof(LocalizationService.CurrentLanguageCode))
            {
                Refresh(DateTimeOffset.Now);
            }
        };
        Refresh(DateTimeOffset.Now);
    }

    public CountdownItem Model => _model;

    public Guid Id => _model.Id;

    public string Title => _model.Title;

    public string SubtitleDisplay => string.IsNullOrWhiteSpace(_model.Subtitle) ? _localization["Countdown.FallbackSubtitle"] : _model.Subtitle;

    public string RemainingPrimary
    {
        get => _remainingPrimary;
        private set => SetProperty(ref _remainingPrimary, value);
    }

    public string RemainingSecondary
    {
        get => _remainingSecondary;
        private set => SetProperty(ref _remainingSecondary, value);
    }

    public string DeadlineDisplay
    {
        get => _deadlineDisplay;
        private set => SetProperty(ref _deadlineDisplay, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public MediaBrush StatusForeground
    {
        get => _statusForeground;
        private set => SetProperty(ref _statusForeground, value);
    }

    public MediaBrush StatusBackground
    {
        get => _statusBackground;
        private set => SetProperty(ref _statusBackground, value);
    }

    public MediaBrush ProgressBrush
    {
        get => _progressBrush;
        private set => SetProperty(ref _progressBrush, value);
    }

    public double ProgressPercent
    {
        get => _progressPercent;
        private set => SetProperty(ref _progressPercent, value);
    }

    public bool IsPinned => _model.IsPinned;

    public bool HasTags => _model.Tags.Count > 0;

    public IReadOnlyList<string> Tags => _model.Tags;

    public bool IsOverdue
    {
        get => _isOverdue;
        private set => SetProperty(ref _isOverdue, value);
    }

    public bool IsUrgent
    {
        get => _isUrgent;
        private set => SetProperty(ref _isUrgent, value);
    }

    public bool IsToday
    {
        get => _isToday;
        private set => SetProperty(ref _isToday, value);
    }

    public bool IsSoon
    {
        get => _isSoon;
        private set => SetProperty(ref _isSoon, value);
    }

    public bool IsSafe
    {
        get => _isSafe;
        private set => SetProperty(ref _isSafe, value);
    }

    public DateTimeOffset TargetAt => _model.TargetAt;

    public void Refresh(DateTimeOffset now)
    {
        Refresh(now, _thresholds);
    }

    public void Refresh(DateTimeOffset now, CountdownThresholds thresholds)
    {
        _thresholds = thresholds;
        var remaining = _model.TargetAt - now;
        var remainingDays = remaining.TotalDays;
        IsOverdue = remainingDays < thresholds.OverdueDays;
        IsToday = !IsOverdue && remainingDays < thresholds.TodayDays;
        IsSoon = !IsOverdue && !IsToday && remainingDays < thresholds.SoonDays;
        IsSafe = !IsOverdue && remainingDays >= thresholds.SafeDays;

        // If users configure a gap between "soon" and "safe", keep that range in "soon"
        // so every countdown always maps to a status badge.
        if (!IsOverdue && !IsToday && !IsSoon && !IsSafe)
        {
            IsSoon = true;
        }

        IsUrgent = IsToday || IsSoon;

        if (IsOverdue)
        {
            StatusText = _localization["Status.Due"];
            StatusForeground = CreateBrush("#FFFFFF");
            StatusBackground = CreateBrush("#F16262");
            ProgressBrush = CreateBrush("#F16262");
            RemainingPrimary = FormatPrimary(-remaining);
            RemainingSecondary = _localization["Countdown.TargetPassed"];
            ProgressPercent = 100;
        }
        else if (IsToday)
        {
            StatusText = _localization["Status.Today"];
            StatusForeground = CreateBrush("#FFFFFF");
            StatusBackground = CreateBrush("#F39A14");
            ProgressBrush = CreateBrush("#F39A14");
            RemainingPrimary = FormatPrimary(remaining);
            RemainingSecondary = _localization.Format("Countdown.LeftSuffix", FormatDetailed(remaining));
            ProgressPercent = CalculateProgress(now);
        }
        else if (IsSoon)
        {
            StatusText = _localization["Status.Soon"];
            StatusForeground = CreateBrush("#8A5300");
            StatusBackground = CreateBrush("#FFE8B8");
            ProgressBrush = CreateBrush("#F5A623");
            RemainingPrimary = FormatPrimary(remaining);
            RemainingSecondary = _localization.Format("Countdown.LeftSuffix", FormatDetailed(remaining));
            ProgressPercent = CalculateProgress(now);
        }
        else
        {
            StatusText = _localization["Status.Safe"];
            StatusForeground = CreateBrush("#1E8D46");
            StatusBackground = CreateBrush("#DFF7E7");
            ProgressBrush = CreateBrush("#39C871");
            RemainingPrimary = FormatPrimary(remaining);
            RemainingSecondary = _localization.Format("Countdown.LeftSuffix", FormatDetailed(remaining));
            ProgressPercent = CalculateProgress(now);
        }

        DeadlineDisplay = _localization.Format("Countdown.Deadline", _model.TargetAt.ToLocalTime());
        OnPropertyChanged(nameof(IsPinned));
        OnPropertyChanged(nameof(HasTags));
        OnPropertyChanged(nameof(Tags));
        OnPropertyChanged(nameof(SubtitleDisplay));
        OnPropertyChanged(nameof(Title));
    }

    public CountdownItem ToModelCopy()
    {
        return new CountdownItem
        {
            Id = _model.Id,
            Title = _model.Title,
            Subtitle = _model.Subtitle,
            TargetAt = _model.TargetAt,
            TimeZoneId = _model.TimeZoneId,
            IsPinned = _model.IsPinned,
            ReminderMinutesBefore = _model.ReminderMinutesBefore,
            ReminderShown = _model.ReminderShown,
            DueShown = _model.DueShown,
            Tags = [.. _model.Tags],
            CreatedAt = _model.CreatedAt
        };
    }

    private double CalculateProgress(DateTimeOffset now)
    {
        var total = _model.TargetAt - _model.CreatedAt;
        if (total <= TimeSpan.Zero)
        {
            return 100;
        }

        var elapsed = now - _model.CreatedAt;
        return Math.Clamp(elapsed.TotalSeconds / total.TotalSeconds * 100, 0, 100);
    }

    private static MediaBrush CreateBrush(string hex)
    {
        return (MediaSolidColorBrush)new MediaBrushConverter().ConvertFrom(hex)!;
    }

    private static string FormatPrimary(TimeSpan span)
    {
        span = span.Duration();
        if (span.TotalDays >= 1)
        {
            return $"{(int)span.TotalDays}d {span.Hours:D2}h";
        }

        if (span.TotalHours >= 1)
        {
            return $"{(int)span.TotalHours:D2}h {span.Minutes:D2}m";
        }

        return $"{Math.Max(0, span.Minutes):D2}m {Math.Max(0, span.Seconds):D2}s";
    }

    private string FormatDetailed(TimeSpan span)
    {
        span = span.Duration();
        if (span.TotalDays >= 1)
        {
            return _localization.Format("Time.Days", (int)span.TotalDays);
        }

        if (span.TotalHours >= 1)
        {
            return _localization.Format("Time.Hours", (int)span.TotalHours);
        }

        if (span.TotalMinutes >= 1)
        {
            return _localization.Format("Time.Minutes", (int)span.TotalMinutes);
        }

        return _localization.Format("Time.Seconds", Math.Max(0, span.Seconds));
    }
}
