using System.Drawing.Drawing2D;
using System.Reflection;

namespace TimeCountdown.Setup;

internal sealed class InstallerForm : Form
{
    private readonly bool _uninstallMode;
    private readonly bool _existingInstall;
    private readonly Image? _brandImage;
    private readonly Panel _brandPanel;
    private readonly Label _eyebrowLabel;
    private readonly Label _titleLabel;
    private readonly Label _bodyLabel;
    private readonly Label _versionLabel;
    private readonly Label _locationLabel;
    private readonly Panel _installPathPanel;
    private readonly TextBox _installPathTextBox;
    private readonly Button _browseInstallPathButton;
    private readonly Label _changesTitleLabel;
    private readonly Label _changesBodyLabel;
    private readonly Label _statusLabel;
    private readonly ProgressBar _progressBar;
    private readonly CheckBox _launchCheckBox;
    private readonly CheckBox _removeDataCheckBox;
    private readonly Button _primaryButton;
    private readonly Button _secondaryButton;
    private readonly Button _closeButton;

    public InstallerForm(bool uninstallMode)
    {
        _uninstallMode = uninstallMode;
        _existingInstall = InstallerContext.IsInstalled;
        _brandImage = LoadBrandImage();

        BackColor = Color.White;
        ClientSize = new Size(920, 560);
        Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(920, 560);
        Text = $"{InstallerContext.ProductName} Setup";
        Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath ?? string.Empty);
        DoubleBuffered = true;

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        Controls.Add(rootLayout);

        _brandPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };
        _brandPanel.Paint += BrandPanelOnPaint;
        rootLayout.Controls.Add(_brandPanel, 0, 0);

        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40, 26, 40, 32),
            Margin = Padding.Empty
        };
        rootLayout.Controls.Add(contentPanel, 1, 0);

        _closeButton = new Button
        {
            Text = "X",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point),
            Size = new Size(40, 36),
            Location = new Point(contentPanel.ClientSize.Width - 40, 0),
            BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(103, 129, 157),
            TabStop = false
        };
        _closeButton.FlatAppearance.BorderSize = 0;
        _closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(235, 244, 253);
        _closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 247, 253);
        _closeButton.Click += (_, _) => Close();
        contentPanel.Controls.Add(_closeButton);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 14,
            Padding = new Padding(0, 4, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < 10; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contentPanel.Controls.Add(layout);

        _eyebrowLabel = CreateLabel(Color.FromArgb(74, 132, 193), new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point), 520);
        _titleLabel = CreateLabel(Color.FromArgb(22, 58, 99), new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point), 520);
        _titleLabel.Margin = new Padding(0, 6, 0, 0);
        _bodyLabel = CreateLabel(Color.FromArgb(89, 112, 136), new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point), 520);
        _bodyLabel.Margin = new Padding(0, 18, 0, 0);
        _versionLabel = CreateLabel(Color.FromArgb(111, 133, 156), new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point), 520);
        _versionLabel.Margin = new Padding(0, 18, 0, 0);
        _locationLabel = CreateLabel(Color.FromArgb(74, 98, 122), new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point), 520);
        _locationLabel.Margin = new Padding(0, 8, 0, 0);

        _installPathTextBox = new TextBox
        {
            Width = 370,
            Height = 32,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            Text = InstallerContext.InstallRoot
        };
        _installPathTextBox.ReadOnly = true;
        _installPathTextBox.TabStop = false;
        _installPathTextBox.Click += BrowseInstallPathButtonOnClick;
        _installPathTextBox.DoubleClick += BrowseInstallPathButtonOnClick;

        _browseInstallPathButton = new Button
        {
            AutoSize = false,
            Size = new Size(84, 32),
            Text = "浏览...",
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(245, 250, 255),
            ForeColor = Color.FromArgb(52, 95, 137),
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(8, 0, 0, 0)
        };
        _browseInstallPathButton.Text = "\u66F4\u6539";
        _browseInstallPathButton.FlatAppearance.BorderColor = Color.FromArgb(206, 224, 242);
        _browseInstallPathButton.FlatAppearance.BorderSize = 1;
        _browseInstallPathButton.Click += BrowseInstallPathButtonOnClick;

        _installPathPanel = new Panel
        {
            Height = 34,
            Margin = new Padding(0, 8, 0, 0),
            Dock = DockStyle.Top
        };
        _installPathPanel.SizeChanged += (_, _) => LayoutInstallPathControls();
        _installPathPanel.Controls.Add(_installPathTextBox);
        _installPathPanel.Controls.Add(_browseInstallPathButton);
        LayoutInstallPathControls();

        _changesTitleLabel = CreateLabel(Color.FromArgb(43, 94, 139), new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point), 520);
        _changesTitleLabel.Margin = new Padding(0, 18, 0, 0);
        _changesBodyLabel = CreateLabel(Color.FromArgb(84, 107, 129), new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point), 520);
        _changesBodyLabel.Margin = new Padding(0, 8, 0, 0);

        _launchCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = $"安装完成后立即启动 {InstallerContext.ProductName}",
            ForeColor = Color.FromArgb(43, 94, 139),
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(0, 18, 0, 0),
            Checked = true
        };

        _removeDataCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "同时删除本地数据（倒计时和设置）",
            ForeColor = Color.FromArgb(43, 94, 139),
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(0, 18, 0, 0),
            Checked = false
        };

        _statusLabel = CreateLabel(Color.FromArgb(68, 120, 174), new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point), 520);
        _statusLabel.Margin = new Padding(0, 18, 0, 0);
        _statusLabel.Visible = false;

        _progressBar = new ProgressBar
        {
            Dock = DockStyle.Top,
            Height = 10,
            Style = ProgressBarStyle.Continuous,
            Margin = new Padding(0, 10, 40, 0),
            Visible = false
        };

        var spacer = new Panel { Dock = DockStyle.Fill };
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 18, 0, 0)
        };

        _primaryButton = new Button
        {
            AutoSize = false,
            Size = new Size(170, 46),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(63, 137, 232),
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(8, 0, 0, 0)
        };
        _primaryButton.FlatAppearance.BorderSize = 0;
        _primaryButton.Click += PrimaryButtonOnClick;

        _secondaryButton = new Button
        {
            AutoSize = false,
            Size = new Size(110, 46),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(74, 98, 122),
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(8, 0, 0, 0)
        };
        _secondaryButton.FlatAppearance.BorderColor = Color.FromArgb(214, 228, 243);
        _secondaryButton.FlatAppearance.BorderSize = 1;
        _secondaryButton.Click += (_, _) => Close();

        buttonPanel.Controls.Add(_primaryButton);
        buttonPanel.Controls.Add(_secondaryButton);

        layout.Controls.Add(_eyebrowLabel, 0, 0);
        layout.Controls.Add(_titleLabel, 0, 1);
        layout.Controls.Add(_bodyLabel, 0, 2);
        layout.Controls.Add(_versionLabel, 0, 3);
        layout.Controls.Add(_locationLabel, 0, 4);
        layout.Controls.Add(_installPathPanel, 0, 5);
        layout.Controls.Add(_changesTitleLabel, 0, 6);
        layout.Controls.Add(_changesBodyLabel, 0, 7);
        layout.Controls.Add(_launchCheckBox, 0, 8);
        layout.Controls.Add(_removeDataCheckBox, 0, 9);
        layout.Controls.Add(_statusLabel, 0, 10);
        layout.Controls.Add(_progressBar, 0, 11);
        layout.Controls.Add(spacer, 0, 12);
        layout.Controls.Add(buttonPanel, 0, 13);

        MouseDown += DragWindow;
        _brandPanel.MouseDown += DragWindow;
        contentPanel.MouseDown += DragWindow;
        layout.MouseDown += DragWindow;

        ShowWelcomeState();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _brandImage?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ShowWelcomeState()
    {
        _eyebrowLabel.Text = _uninstallMode ? "UNINSTALLER" : (_existingInstall ? "UPDATE INSTALLER" : "SETUP");
        _titleLabel.Text = _uninstallMode
            ? $"卸载 {InstallerContext.ProductName}"
            : (_existingInstall ? $"升级你的 {InstallerContext.ProductName}" : $"安装 {InstallerContext.ProductName}");

        _bodyLabel.Text = _uninstallMode
            ? "安装器会移除软件本体、桌面快捷方式和开始菜单入口。默认会保留本地倒计时与设置数据；如果你希望彻底清空，也可以勾选删除本地数据。"
            : $"{InstallerContext.ProductName} 是一款面向 Windows 11 的轻量级桌面倒计时应用，强调低内存占用、简洁界面和始终可见的桌面浮层体验。";

        _versionLabel.Text = $"版本 {InstallerContext.ProductVersion}  |  适用于 Windows 11 x64";
        _locationLabel.Text = _uninstallMode
            ? $"当前安装位置{Environment.NewLine}{InstallerContext.InstallRoot}"
            : "安装位置（可自定义）";
        _installPathTextBox.Text = InstallerContext.InstallRoot;
        _changesTitleLabel.Text = _uninstallMode ? string.Empty : $"{InstallerContext.ProductVersion} 改动细则";
        _changesBodyLabel.Text = _uninstallMode ? string.Empty : InstallerReleaseInfo.BuildHighlightsText();
        _installPathPanel.Visible = !_uninstallMode;
        _changesTitleLabel.Visible = !_uninstallMode;
        _changesBodyLabel.Visible = !_uninstallMode;
        _launchCheckBox.Visible = !_uninstallMode;
        _removeDataCheckBox.Visible = _uninstallMode;
        _statusLabel.Visible = false;
        _progressBar.Visible = false;
        _secondaryButton.Visible = true;
        _secondaryButton.Enabled = true;
        _secondaryButton.Text = "取消";
        _primaryButton.Text = _uninstallMode ? "立即卸载" : (_existingInstall ? "立即更新" : "立即安装");
        _primaryButton.Enabled = true;
    }

    private async void PrimaryButtonOnClick(object? sender, EventArgs e)
    {
        ToggleBusy(true);

        var progress = new Progress<InstallerProgress>(value =>
        {
            _statusLabel.Text = $"{value.Title}{Environment.NewLine}{value.Detail}";
            _progressBar.Value = Math.Clamp(value.Percent, 0, 100);
        });

        try
        {
            if (_uninstallMode)
            {
                ShowProgressState($"正在卸载 {InstallerContext.ProductName}", "正在清理快捷方式、软件本体和可选的本地数据。");
                await RunStaTask(() => InstallerEngine.Uninstall(
                    new InstallOptions
                    {
                        RemoveLocalData = _removeDataCheckBox.Checked
                    },
                    progress));
                ShowCompleteState(
                    "卸载完成",
                    _removeDataCheckBox.Checked
                        ? $"{InstallerContext.ProductName} 已从当前用户目录移除，并删除了本地倒计时和设置数据。"
                        : $"{InstallerContext.ProductName} 已从当前用户目录移除。本地数据仍被保留，后续重新安装后可以继续恢复使用。",
                    "关闭");
                return;
            }

            ShowProgressState(
                _existingInstall ? $"正在更新 {InstallerContext.ProductName}" : $"正在安装 {InstallerContext.ProductName}",
                "正在准备桌面倒计时应用和安装资源。");

            var selectedInstallDirectory = GetSelectedInstallDirectory();
            await RunStaTask(() => InstallerEngine.Install(
                new InstallOptions
                {
                    LaunchAfterInstall = _launchCheckBox.Checked,
                    InstallDirectory = selectedInstallDirectory
                },
                progress));

            ShowCompleteState(
                _launchCheckBox.Checked ? "安装完成，已为你启动应用" : "安装完成",
                _launchCheckBox.Checked
                    ? $"你现在可以开始使用 {InstallerContext.ProductName} 添加截止日期、固定重要卡片，并在设置中启用桌面层模式。"
                    : $"{InstallerContext.ProductName} 已成功安装。你可以从桌面快捷方式或开始菜单随时启动它。",
                "完成");
        }
        catch (Exception ex)
        {
            ToggleBusy(false);
            MessageBox.Show(
                $"安装过程中出现问题：{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                InstallerContext.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            ShowWelcomeState();
        }
    }

    private void ShowProgressState(string title, string detail)
    {
        _titleLabel.Text = title;
        _bodyLabel.Text = detail;
        _versionLabel.Text = string.Empty;
        _locationLabel.Text = string.Empty;
        _installPathPanel.Visible = false;
        _changesTitleLabel.Visible = false;
        _changesBodyLabel.Visible = false;
        _launchCheckBox.Visible = false;
        _removeDataCheckBox.Visible = false;
        _statusLabel.Visible = true;
        _progressBar.Visible = true;
        _progressBar.Value = 0;
        _statusLabel.Text = $"正在开始...{Environment.NewLine}请保持此窗口打开。";
        _secondaryButton.Enabled = false;
        _primaryButton.Enabled = false;
    }

    private void ShowCompleteState(string title, string body, string primaryText)
    {
        ToggleBusy(false);
        _eyebrowLabel.Text = "DONE";
        _titleLabel.Text = title;
        _bodyLabel.Text = body;
        _versionLabel.Text = string.Empty;
        _locationLabel.Text = string.Empty;
        _installPathPanel.Visible = false;
        _changesTitleLabel.Visible = false;
        _changesBodyLabel.Visible = false;
        _launchCheckBox.Visible = false;
        _removeDataCheckBox.Visible = false;
        _statusLabel.Visible = false;
        _progressBar.Visible = false;
        _secondaryButton.Visible = false;
        _primaryButton.Text = primaryText;
        _primaryButton.Enabled = true;
        _primaryButton.Click -= PrimaryButtonOnClick;
        _primaryButton.Click += (_, _) => Close();
    }

    private void ToggleBusy(bool busy)
    {
        UseWaitCursor = busy;
        _primaryButton.Enabled = !busy;
        _secondaryButton.Enabled = !busy;
        _closeButton.Enabled = !busy;
    }

    private async Task RunStaTask(Action action)
    {
        var completion = new TaskCompletionSource<object?>();
        var thread = new Thread(() =>
        {
            try
            {
                action();
                completion.SetResult(null);
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        await completion.Task;
    }

    private void BrandPanelOnPaint(object? sender, PaintEventArgs e)
    {
        var bounds = _brandPanel.ClientRectangle;
        using var brush = new LinearGradientBrush(bounds, Color.FromArgb(235, 245, 255), Color.FromArgb(193, 221, 252), 135f);
        e.Graphics.FillRectangle(brush, bounds);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using var overlayBrush = new SolidBrush(Color.FromArgb(70, 255, 255, 255));
        e.Graphics.FillEllipse(overlayBrush, -86, 18, 220, 220);
        e.Graphics.FillEllipse(overlayBrush, 116, 304, 176, 176);

        if (_brandImage is not null)
        {
            e.Graphics.DrawImage(_brandImage, new Rectangle(28, 34, 80, 80));
        }

        using var badgeBrush = new SolidBrush(Color.FromArgb(245, 251, 255));
        using var badgeTextBrush = new SolidBrush(Color.FromArgb(69, 123, 191));
        using var titleBrush = new SolidBrush(Color.FromArgb(23, 70, 117));
        using var bodyBrush = new SolidBrush(Color.FromArgb(66, 105, 145));
        using var footBrush = new SolidBrush(Color.FromArgb(95, 121, 149));

        e.Graphics.FillRoundedRectangle(badgeBrush, new Rectangle(122, 54, 164, 34), 17);
        using var badgeFont = new Font("Segoe UI Semibold", 8.8f, FontStyle.Bold);
        using var titleFont = new Font("Segoe UI Semibold", 22f, FontStyle.Bold);
        using var bodyFont = new Font("Segoe UI", 10.3f);
        using var footFont = new Font("Segoe UI", 9.5f);

        e.Graphics.DrawString(InstallerContext.ProductName, badgeFont, badgeTextBrush, new RectangleF(132, 60, 144, 20));
        e.Graphics.DrawString("轻量、常驻、专注的桌面倒计时助手", titleFont, titleBrush, new RectangleF(28, 132, 258, 118));
        e.Graphics.DrawString("- 轻量级桌面原生应用\r\n- 常驻悬浮，不打扰也不缺席\r\n- 简洁卡片式 UI，让计划一直在眼前", bodyFont, bodyBrush, new RectangleF(30, 258, 252, 134));
        e.Graphics.DrawString("适合论文投稿、项目里程碑、考试日程和个人计划管理。", footFont, footBrush, new RectangleF(30, 414, 248, 46));
    }

    private static Label CreateLabel(Color color, Font font, int maxWidth)
    {
        return new Label
        {
            AutoSize = true,
            MaximumSize = new Size(maxWidth, 0),
            ForeColor = color,
            Font = font,
            Margin = new Padding(0)
        };
    }

    private static Image? LoadBrandImage()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("AppIcon-256.png", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        buffer.Position = 0;
        return Image.FromStream(buffer);
    }

    private void LayoutInstallPathControls()
    {
        var availableWidth = _installPathPanel.ClientSize.Width;
        if (availableWidth <= 0)
        {
            return;
        }

        const int spacing = 8;
        var buttonWidth = _browseInstallPathButton.Width;
        var textBoxWidth = Math.Max(160, availableWidth - buttonWidth - spacing);
        _installPathTextBox.Location = new Point(0, 0);
        _installPathTextBox.Width = textBoxWidth;
        _browseInstallPathButton.Location = new Point(textBoxWidth + spacing, 0);
    }

    private void BrowseInstallPathButtonOnClick(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = $"选择 {InstallerContext.ProductName} 的安装目录",
            UseDescriptionForTitle = true,
            SelectedPath = string.IsNullOrWhiteSpace(_installPathTextBox.Text)
                ? InstallerContext.DefaultInstallRoot
                : _installPathTextBox.Text
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _installPathTextBox.Text = dialog.SelectedPath;
        }
    }

    private string GetSelectedInstallDirectory()
    {
        if (_uninstallMode)
        {
            return InstallerContext.InstallRoot;
        }

        var rawPath = _installPathTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            throw new InvalidOperationException("安装目录不能为空。");
        }

        return Path.GetFullPath(Environment.ExpandEnvironmentVariables(rawPath));
    }

    private void DragWindow(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        NativeMethods.ReleaseCapture();
        NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
    }

    private static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern nint SendMessage(nint hWnd, int msg, int wParam, int lParam);
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int radius)
    {
        using var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        graphics.FillPath(brush, path);
    }
}
