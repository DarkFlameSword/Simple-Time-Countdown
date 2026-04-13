using System.Windows;
using TimeCountdown;
using TimeCountdown.ViewModels;

namespace TimeCountdown.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(MainWindowViewModel viewModel, MainWindow mainWindow)
    {
        InitializeComponent();
        DataContext = viewModel;
        Owner = mainWindow;
    }

    private void ResetPlacement_Click(object sender, RoutedEventArgs e)
    {
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.ResetWindowPlacement();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
