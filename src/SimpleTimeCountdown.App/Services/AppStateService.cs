using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TimeCountdown.Models;

namespace TimeCountdown.Services;

public sealed class AppStateService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public string StateDirectoryPath { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimeCountdown");

    public string StateFilePath => Path.Combine(StateDirectoryPath, "state.json");

    public AppState Load()
    {
        try
        {
            Directory.CreateDirectory(StateDirectoryPath);
            if (!File.Exists(StateFilePath))
            {
                return CreateDefaultState();
            }

            var json = File.ReadAllText(StateFilePath);
            var state = JsonSerializer.Deserialize<AppState>(json, SerializerOptions);
            return state ?? CreateDefaultState();
        }
        catch
        {
            return CreateDefaultState();
        }
    }

    public void Save(AppState state)
    {
        Directory.CreateDirectory(StateDirectoryPath);
        var json = JsonSerializer.Serialize(state, SerializerOptions);
        File.WriteAllText(StateFilePath, json);
    }

    private static AppState CreateDefaultState()
    {
        var now = DateTimeOffset.Now;
        return new AppState
        {
            Items =
            [
                new CountdownItem
                {
                    Title = "Project Demo",
                    Subtitle = "Internal milestone",
                    TargetAt = now.AddHours(5),
                    IsPinned = true,
                    ReminderMinutesBefore = 60,
                    Tags = ["Work", "Urgent"],
                    CreatedAt = now.AddDays(-4)
                },
                new CountdownItem
                {
                    Title = "NeurIPS 2026",
                    Subtitle = "Abstract deadline",
                    TargetAt = now.AddDays(25).AddHours(2),
                    ReminderMinutesBefore = 24 * 60,
                    Tags = ["Conference", "ML"],
                    CreatedAt = now.AddDays(-15)
                },
                new CountdownItem
                {
                    Title = "Renew passport",
                    Subtitle = "Personal admin",
                    TargetAt = now.AddDays(44).AddHours(6),
                    ReminderMinutesBefore = 3 * 24 * 60,
                    Tags = ["Personal"],
                    CreatedAt = now.AddDays(-5)
                }
            ]
        };
    }
}
