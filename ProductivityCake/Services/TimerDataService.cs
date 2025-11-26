using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

/// <summary>
/// Service for persisting timer settings and statistics
/// </summary>
public class TimerDataService
{
    private readonly string _settingsFilePath;
    private readonly string _statisticsFilePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions;

    public TimerDataService()
    {
        // Use user's home directory for data storage (works with AppImage)
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dataDirectory = Path.Combine(homeDir, ".local", "share", "ProductivityCake", "Data");
        Directory.CreateDirectory(dataDirectory);
        
        _settingsFilePath = Path.Combine(dataDirectory, "timer-settings.json");
        _statisticsFilePath = Path.Combine(dataDirectory, "timer-statistics.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Load timer settings from disk
    /// </summary>
    public async Task<TimerSettings> LoadSettingsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_settingsFilePath))
                return new TimerSettings();

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new TimerSettings();

            var settings = JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.TimerSettings);
            return settings ?? new TimerSettings();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading timer settings: {ex.Message}");
            return new TimerSettings();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Save timer settings to disk
    /// </summary>
    public async Task<bool> SaveSettingsAsync(TimerSettings settings)
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(settings, AppJsonSerializerContext.Default.TimerSettings);
            await File.WriteAllTextAsync(_settingsFilePath, json);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving timer settings: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Load timer statistics from disk
    /// </summary>
    public async Task<TimerStatistics> LoadStatisticsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(_statisticsFilePath))
                return new TimerStatistics();

            var json = await File.ReadAllTextAsync(_statisticsFilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new TimerStatistics();

            var statistics = JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.TimerStatistics);
            return statistics ?? new TimerStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading timer statistics: {ex.Message}");
            return new TimerStatistics();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Save timer statistics to disk
    /// </summary>
    public async Task<bool> SaveStatisticsAsync(TimerStatistics statistics)
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(statistics, AppJsonSerializerContext.Default.TimerStatistics);
            await File.WriteAllTextAsync(_statisticsFilePath, json);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving timer statistics: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
