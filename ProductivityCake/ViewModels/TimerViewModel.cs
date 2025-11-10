using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Models;

namespace ProductivityCake.ViewModels;

public partial class TimerViewModel : ViewModelBase, IDisposable
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WorkDuration))]
    private int _workSessionMinutes = 25;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShortBreakDuration))]
    private int _shortBreakMinutes = 5;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LongBreakDuration))]
    private int _longBreakMinutes = 15;
    private const int _pomodorosBeforeLongBreak = 4;
    
    // Partial methods to handle property changes
    partial void OnWorkSessionMinutesChanged(int value)
    {
        // Update timer display if we're in a work state and timer is not running
        if (!IsRunning && (CurrentState == TimerState.Work || CurrentState == TimerState.LongWork))
        {
            TimeRemaining = WorkDuration;
        }
    }
    
    partial void OnShortBreakMinutesChanged(int value)
    {
        // Update timer display if we're in short break state and timer is not running
        if (!IsRunning && CurrentState == TimerState.ShortBreak)
        {
            TimeRemaining = ShortBreakDuration;
        }
    }
    
    partial void OnLongBreakMinutesChanged(int value)
    {
        // Update timer display if we're in long break state and timer is not running
        if (!IsRunning && CurrentState == TimerState.LongBreak)
        {
            TimeRemaining = LongBreakDuration;
        }
    }
    
    private Timer? _timer;
    private DateTime _endTime;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimeDisplay), nameof(Progress))]
    private TimeSpan _timeRemaining;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentSessionDisplay))]
    private int _completedPomodoros;
    
    [ObservableProperty]
    private bool _isRunning;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWorkState), nameof(IsBreakState), nameof(StateDisplayName), 
        nameof(IsShortWork), nameof(IsLongWork), nameof(IsShortBreak), nameof(IsLongBreak))]
    private TimerState _currentState;
    
    [ObservableProperty]
    private bool _notificationsEnabled = true;
    
    [ObservableProperty]
    private bool _alwaysOnTop = false;
    
    public TimeSpan WorkDuration => TimeSpan.FromMinutes(WorkSessionMinutes);
    public TimeSpan ShortBreakDuration => TimeSpan.FromMinutes(ShortBreakMinutes);
    public TimeSpan LongBreakDuration => TimeSpan.FromMinutes(LongBreakMinutes);
    
    // Statistics
    [ObservableProperty]
    private TimeSpan _totalWorkTimeToday;
    
    [ObservableProperty]
    private TimeSpan _totalBreakTimeToday;
    
    [ObservableProperty]
    private TimeSpan _totalWorkTimeWeek;
    
    [ObservableProperty]
    private TimeSpan _totalWorkTimeMonth;
    
    [ObservableProperty]
    private int _sessionsToday;
    
    [ObservableProperty]
    private int _sessionsWeek;
    
    [ObservableProperty]
    private int _sessionsMonth;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TodayStatDisplay), nameof(WeekStatDisplay), nameof(MonthStatDisplay))]
    private int _statisticsViewIndex = 0; // 0 = Time, 1 = Sessions
    
    [ObservableProperty]
    private int _graphPeriodIndex = 0; // 0 = Days, 1 = Weeks, 2 = Months, 3 = Years
    
    [ObservableProperty]
    private int _currentStreak;
    
    [ObservableProperty]
    private int _longestStreak;
    
    [ObservableProperty]
    private DateTime _lastSessionDate;
    
    [ObservableProperty]
    private ObservableCollection<HeatmapWeek> _heatmapWeeks = new();
    
    [ObservableProperty]
    private ObservableCollection<HeatmapMonth> _heatmapMonths = new();
    
    private Dictionary<DateTime, int> _dailySessionCounts = new();
    
    private DateTime _sessionStartTime;
    private DateTime _todayDate;
    
    public TimerViewModel()
    {
        _todayDate = DateTime.Today;
        _lastSessionDate = DateTime.MinValue;
        LoadTodayStatistics();
        GenerateHeatmap();
        SwitchToWork();
    }
    
    public string TimeDisplay => $"{(int)TimeRemaining.TotalMinutes:D2}:{TimeRemaining.Seconds:D2}";
    
    public string StateDisplayName => CurrentState switch
    {
        TimerState.Work => "Work Session",
        TimerState.LongWork => "Work Session",
        TimerState.ShortBreak => "Short Break",
        TimerState.LongBreak => "Long Break",
        _ => "Unknown"
    };
    
    public string CurrentSessionDisplay
    {
        get
        {
            // Calculate current session (1-4)
            int currentSession = (CompletedPomodoros % _pomodorosBeforeLongBreak) + 1;
            return $"{currentSession}/{_pomodorosBeforeLongBreak} Sessions";
        }
    }
    
    public double Progress
    {
        get
        {
            var totalDuration = CurrentState switch
            {
                TimerState.Work => WorkDuration,
                TimerState.LongWork => WorkDuration,
                TimerState.ShortBreak => ShortBreakDuration,
                TimerState.LongBreak => LongBreakDuration,
                _ => WorkDuration
            };
            
            return 1.0 - (TimeRemaining.TotalMilliseconds / totalDuration.TotalMilliseconds);
        }
    }
    
    public bool IsWorkState => CurrentState == TimerState.Work || CurrentState == TimerState.LongWork;
    public bool IsBreakState => CurrentState == TimerState.ShortBreak || CurrentState == TimerState.LongBreak;
    
    // Specific mode checks for button highlighting
    public bool IsShortWork => CurrentState == TimerState.Work;
    public bool IsLongWork => CurrentState == TimerState.LongWork;
    public bool IsShortBreak => CurrentState == TimerState.ShortBreak;
    public bool IsLongBreak => CurrentState == TimerState.LongBreak;
    
    [RelayCommand]
    private void SwitchToWork()
    {
        StopTimer();
        CurrentState = TimerState.Work;
        TimeRemaining = WorkDuration;
    }
    
    [RelayCommand]
    private void SwitchToShortBreak()
    {
        StopTimer();
        CurrentState = TimerState.ShortBreak;
        TimeRemaining = ShortBreakDuration;
    }
    
    [RelayCommand]
    private void SwitchToLongBreak()
    {
        StopTimer();
        CurrentState = TimerState.LongBreak;
        TimeRemaining = LongBreakDuration;
    }
    
    [RelayCommand]
    private void StartTestTimer()
    {
        StopTimer();
        TimeRemaining = TimeSpan.FromSeconds(5);
        StartTimer();
    }
    
    [RelayCommand]
    private void ToggleTimer()
    {
        if (IsRunning)
        {
            PauseTimer();
        }
        else
        {
            StartTimer();
        }
    }
    
    private void StartTimer()
    {
        if (IsRunning) return;
        
        _sessionStartTime = DateTime.Now;
        _endTime = DateTime.Now.Add(TimeRemaining);
        _timer = new Timer(TimerCallback, null, 0, 1000);
        IsRunning = true;
        
        CheckAndResetDailyStats();
    }
    
    private void PauseTimer()
    {
        if (IsRunning)
        {
            // Track time when pausing
            var sessionDuration = DateTime.Now - _sessionStartTime;
            TrackSessionTime(sessionDuration);
        }
        
        _timer?.Dispose();
        _timer = null;
        IsRunning = false;
    }
    
    private void StopTimer()
    {
        PauseTimer();
        TimeRemaining = CurrentState switch
        {
            TimerState.Work => WorkDuration,
            TimerState.LongWork => WorkDuration,
            TimerState.ShortBreak => ShortBreakDuration,
            TimerState.LongBreak => LongBreakDuration,
            _ => WorkDuration
        };
    }
    
    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        TimeRemaining = CurrentState switch
        {
            TimerState.Work => WorkDuration,
            TimerState.LongWork => WorkDuration,
            TimerState.ShortBreak => ShortBreakDuration,
            TimerState.LongBreak => LongBreakDuration,
            _ => WorkDuration
        };
    }
    
    [RelayCommand]
    private void AddTime()
    {
        var wasRunning = IsRunning;
        if (wasRunning)
        {
            PauseTimer();
        }
        
        TimeRemaining = TimeRemaining.Add(TimeSpan.FromMinutes(5));
        
        if (wasRunning)
        {
            StartTimer();
        }
    }
    
    [RelayCommand]
    private void SubtractTime()
    {
        var wasRunning = IsRunning;
        if (wasRunning)
        {
            PauseTimer();
        }
        
        var newTime = TimeRemaining.Subtract(TimeSpan.FromMinutes(5));
        TimeRemaining = newTime > TimeSpan.Zero ? newTime : TimeSpan.FromMinutes(1);
        
        if (wasRunning)
        {
            StartTimer();
        }
    }
    
    [RelayCommand]
    private void SkipSession()
    {
        StopTimer();
        
        // Skip to the next logical session
        if (IsWorkState)
        {
            // After work, go to break
            if (CompletedPomodoros > 0 && CompletedPomodoros % _pomodorosBeforeLongBreak == 0 && CurrentState == TimerState.Work)
            {
                SwitchToLongBreak();
            }
            else
            {
                SwitchToShortBreak();
            }
        }
        else
        {
            // After break, go back to work
            SwitchToWork();
        }
    }
    
    [RelayCommand]
    private void OpenGitHub()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = "https://github.com/amuza2/ProductivityCake",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to open GitHub: {ex.Message}");
        }
    }
    
    private void TimerCallback(object? state)
    {
        TimeRemaining = _endTime - DateTime.Now;
        
        if (TimeRemaining <= TimeSpan.Zero)
        {
            TimeRemaining = TimeSpan.Zero;
            PauseTimer();
            
            if (CurrentState == TimerState.Work || CurrentState == TimerState.LongWork)
            {
                // Only count pomodoros for standard 25-minute work sessions
                if (CurrentState == TimerState.Work)
                {
                    CompletedPomodoros++;
                    SessionsToday++;
                    SessionsWeek++;
                    SessionsMonth++;
                    UpdateHeatmapForToday();
                }
                
                // Auto-start break after work
                if (CompletedPomodoros > 0 && CompletedPomodoros % _pomodorosBeforeLongBreak == 0 && CurrentState == TimerState.Work)
                {
                    SwitchToLongBreak();
                }
                else
                {
                    SwitchToShortBreak();
                }
                
                // Auto-start the break timer
                StartTimer();
            }
            else
            {
                // Auto-start work after break
                SwitchToWork();
            }
            
            // Send notification
            SendNotification();
        }
    }
    
    private void SendNotification()
    {
        if (!NotificationsEnabled)
            return;
        
        Process? soundProcess = null;
        
        try
        {
            var message = CurrentState switch
            {
                TimerState.Work => "Work session completed! Time for a break.",
                TimerState.LongWork => "Long work session completed! Time for a break.",
                TimerState.ShortBreak => "Short break completed! Ready to work?",
                TimerState.LongBreak => "Long break completed! Ready to work?",
                _ => "Timer completed!"
            };
            
            var title = "Pomodoro Timer";
            
            // Start playing sound
            soundProcess = PlayAlarmSound();
            
            // Use notify-send for Linux notifications
            var notificationProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "notify-send",
                    Arguments = $"\"{title}\" \"{message}\" --icon=dialog-information --urgency=normal --expire-time=10000 --wait",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            notificationProcess.Start();
            notificationProcess.WaitForExit(); // Wait for notification to be dismissed
            
            // Stop sound when notification is dismissed
            if (soundProcess != null && !soundProcess.HasExited)
            {
                soundProcess.Kill();
                soundProcess.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send notification: {ex.Message}");
            
            // Make sure to clean up sound process
            if (soundProcess != null && !soundProcess.HasExited)
            {
                try { soundProcess.Kill(); soundProcess.Dispose(); } catch { }
            }
        }
    }
    
    private Process? PlayAlarmSound()
    {
        try
        {
            // Try multiple possible locations for the sound file
            var possiblePaths = new[]
            {
                // Published version (alarm.mp3 in same directory as executable)
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarm.mp3"),
                // Development version (in Assets folder)
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "alarm.mp3"),
                // Source directory (for development)
                System.IO.Path.Combine(
                    System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "",
                    "Assets", "alarm.mp3")
            };
            
            string? soundPath = null;
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    soundPath = path;
                    break;
                }
            }
            
            if (soundPath == null)
            {
                Console.WriteLine($"Sound file not found. Tried paths:");
                foreach (var path in possiblePaths)
                {
                    Console.WriteLine($"  - {path}");
                }
                return null;
            }
            
            Console.WriteLine($"Playing sound from: {soundPath}");
            
            // Try multiple audio players in order of preference
            var players = new (string player, string args)[]
            {
                ("paplay", $"--volume=32768 \"{soundPath}\""), // PulseAudio (most common)
                ("ffplay", $"-nodisp -autoexit -loop 3 -volume 50 \"{soundPath}\""), // FFmpeg
                ("mpg123", $"-q --loop -1 \"{soundPath}\""), // mpg123
                ("cvlc", $"--play-and-exit --loop \"{soundPath}\"") // VLC
            };
            
            foreach (var (player, args) in players)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = player,
                            Arguments = args,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    
                    process.Start();
                    Console.WriteLine($"Successfully started audio playback with {player}");
                    return process;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {player}: {ex.Message}");
                    // Try next player
                }
            }
            
            Console.WriteLine("No audio player found. Please install paplay (pulseaudio-utils), ffplay (ffmpeg), mpg123, or vlc.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to play sound: {ex.Message}");
            return null;
        }
    }
    
    // Statistics Methods
    private void LoadTodayStatistics()
    {
        // TODO: Load from persistent storage
        // For now, reset daily
        TotalWorkTimeToday = TimeSpan.Zero;
        TotalBreakTimeToday = TimeSpan.Zero;
        CurrentStreak = 0;
        LongestStreak = 0;
    }
    
    private void CheckAndResetDailyStats()
    {
        if (DateTime.Today != _todayDate)
        {
            _todayDate = DateTime.Today;
            TotalWorkTimeToday = TimeSpan.Zero;
            TotalBreakTimeToday = TimeSpan.Zero;
        }
    }
    
    private void TrackSessionTime(TimeSpan duration)
    {
        if (IsWorkState)
        {
            TotalWorkTimeToday += duration;
        }
        else
        {
            TotalBreakTimeToday += duration;
        }
        
        UpdateStreak();
    }
    
    private void UpdateStreak()
    {
        var today = DateTime.Today;
        
        if (LastSessionDate == DateTime.MinValue)
        {
            // First session ever
            CurrentStreak = 1;
            LastSessionDate = today;
        }
        else if (LastSessionDate == today)
        {
            // Already worked today, streak continues
            return;
        }
        else if (LastSessionDate == today.AddDays(-1))
        {
            // Worked yesterday, increment streak
            CurrentStreak++;
            LastSessionDate = today;
        }
        else
        {
            // Streak broken, reset
            CurrentStreak = 1;
            LastSessionDate = today;
        }
        
        if (CurrentStreak > LongestStreak)
        {
            LongestStreak = CurrentStreak;
        }
    }
    
    public string TotalWorkTimeTodayDisplay => FormatTimeSpan(TotalWorkTimeToday);
    
    public string TotalBreakTimeTodayDisplay => FormatTimeSpan(TotalBreakTimeToday);
    
    public string TotalWorkTimeWeekDisplay => FormatTimeSpan(TotalWorkTimeWeek);
    
    public string TotalWorkTimeMonthDisplay => FormatTimeSpan(TotalWorkTimeMonth);
    
    public string CurrentMonthName => DateTime.Now.ToString("MMMM");
    
    public string TodayStatDisplay => StatisticsViewIndex == 0 ? TotalWorkTimeTodayDisplay : $"{SessionsToday}";
    
    public string WeekStatDisplay => StatisticsViewIndex == 0 ? TotalWorkTimeWeekDisplay : $"{SessionsWeek}";
    
    public string MonthStatDisplay => StatisticsViewIndex == 0 ? TotalWorkTimeMonthDisplay : $"{SessionsMonth}";
    
    private string FormatTimeSpan(TimeSpan time)
    {
        if (time.TotalMinutes < 60)
        {
            // Less than 60 minutes: show as MM:SS
            return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            // 60 minutes or more: show as Hh MMm
            int hours = (int)time.TotalHours;
            int minutes = time.Minutes;
            return $"{hours}h {minutes:D2}m";
        }
    }
    
    private void GenerateHeatmap()
    {
        HeatmapWeeks.Clear();
        HeatmapMonths.Clear();
        
        var today = DateTime.Today;
        var startDate = today.AddDays(-364); // Show last 365 days
        
        // Align to start of week (Sunday)
        while (startDate.DayOfWeek != DayOfWeek.Sunday)
        {
            startDate = startDate.AddDays(-1);
        }
        
        var currentDate = startDate;
        var weeks = new List<HeatmapWeek>();
        var monthLabels = new Dictionary<string, int>(); // Track month positions
        int weekIndex = 0;
        
        while (currentDate <= today)
        {
            var week = new HeatmapWeek();
            
            // Track month changes for labels
            if (currentDate.Day <= 7 || weekIndex == 0)
            {
                var monthKey = currentDate.ToString("MMM");
                if (!monthLabels.ContainsKey(monthKey))
                {
                    monthLabels[monthKey] = weekIndex;
                }
            }
            
            for (int i = 0; i < 7; i++)
            {
                if (currentDate > today)
                {
                    // Add empty day for future dates
                    week.Days.Add(new HeatmapDay
                    {
                        Date = currentDate,
                        SessionCount = 0,
                        Color = "#374151",
                        Tooltip = ""
                    });
                }
                else
                {
                    var sessionCount = _dailySessionCounts.ContainsKey(currentDate.Date) 
                        ? _dailySessionCounts[currentDate.Date] 
                        : 0;
                    
                    var color = GetHeatmapColor(sessionCount);
                    var tooltip = $"{currentDate:MMM dd, yyyy}: {sessionCount} session{(sessionCount != 1 ? "s" : "")}";
                    
                    week.Days.Add(new HeatmapDay
                    {
                        Date = currentDate,
                        SessionCount = sessionCount,
                        Color = color,
                        Tooltip = tooltip
                    });
                }
                
                currentDate = currentDate.AddDays(1);
            }
            
            weeks.Add(week);
            weekIndex++;
        }
        
        foreach (var week in weeks)
        {
            HeatmapWeeks.Add(week);
        }
        
        // Generate month labels
        var previousMonth = "";
        var monthStartWeek = 0;
        
        for (int i = 0; i < weeks.Count; i++)
        {
            var firstDay = weeks[i].Days.FirstOrDefault(d => d.Date <= today);
            if (firstDay != null)
            {
                var currentMonth = firstDay.Date.ToString("MMM");
                
                if (currentMonth != previousMonth && i > 0)
                {
                    // Add the previous month label
                    var weeksInMonth = i - monthStartWeek;
                    if (weeksInMonth > 0 && !string.IsNullOrEmpty(previousMonth))
                    {
                        HeatmapMonths.Add(new HeatmapMonth
                        {
                            MonthName = previousMonth,
                            Width = weeksInMonth * 15 // 12px square + 3px spacing
                        });
                    }
                    monthStartWeek = i;
                }
                
                previousMonth = currentMonth;
            }
        }
        
        // Add the last month
        if (!string.IsNullOrEmpty(previousMonth))
        {
            var weeksInMonth = weeks.Count - monthStartWeek;
            HeatmapMonths.Add(new HeatmapMonth
            {
                MonthName = previousMonth,
                Width = weeksInMonth * 15
            });
        }
    }
    
    private string GetHeatmapColor(int sessionCount)
    {
        return sessionCount switch
        {
            0 => "#374151",      // No sessions - gray
            1 => "#0E4429",      // 1 session - lightest green
            2 or 3 => "#006D32", // 2-3 sessions - light green
            4 or 5 => "#26A641", // 4-5 sessions - medium green
            _ => "#39D353"       // 6+ sessions - bright green
        };
    }
    
    private void UpdateHeatmapForToday()
    {
        var today = DateTime.Today;
        
        if (!_dailySessionCounts.ContainsKey(today))
        {
            _dailySessionCounts[today] = 0;
        }
        
        _dailySessionCounts[today]++;
        
        // Regenerate heatmap to reflect the update
        GenerateHeatmap();
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}

public enum TimerState
{
    Work,
    LongWork,
    ShortBreak,
    LongBreak
}