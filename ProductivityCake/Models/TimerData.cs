using System;
using System.Collections.Generic;

namespace ProductivityCake.Models;

/// <summary>
/// Represents the timer settings that can be configured by the user
/// </summary>
public class TimerSettings
{
    public int WorkSessionMinutes { get; set; } = 25;
    public int ShortBreakMinutes { get; set; } = 5;
    public int LongBreakMinutes { get; set; } = 15;
    public bool NotificationsEnabled { get; set; } = true;
    public bool AlwaysOnTop { get; set; } = false;
    public int SelectedSoundIndex { get; set; } = 0;
    public int SoundVolume { get; set; } = 50;
    public int NotificationTimeoutSeconds { get; set; } = 10;
}

/// <summary>
/// Represents the timer statistics and progress data
/// </summary>
public class TimerStatistics
{
    public DateTime LastSavedDate { get; set; } = DateTime.Today;
    
    // Daily statistics
    public TimeSpan TotalWorkTimeToday { get; set; } = TimeSpan.Zero;
    public TimeSpan TotalBreakTimeToday { get; set; } = TimeSpan.Zero;
    public int SessionsToday { get; set; } = 0;
    
    // Weekly statistics
    public TimeSpan TotalWorkTimeWeek { get; set; } = TimeSpan.Zero;
    public int SessionsWeek { get; set; } = 0;
    public DateTime WeekStartDate { get; set; } = DateTime.Today;
    
    // Monthly statistics
    public TimeSpan TotalWorkTimeMonth { get; set; } = TimeSpan.Zero;
    public int SessionsMonth { get; set; } = 0;
    public DateTime MonthStartDate { get; set; } = DateTime.Today;
    
    // Session tracking
    public int CompletedPomodoros { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateTime LastSessionDate { get; set; } = DateTime.MinValue;
    
    // Heatmap data - stores session counts per day
    public Dictionary<DateTime, int> DailySessionCounts { get; set; } = new();
}
