using System;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Models;

namespace ProductivityCake.ViewModels;

public partial class TimerViewModel : ViewModelBase, IDisposable
{
    private readonly TimeSpan _workDuration = TimeSpan.FromMinutes(25);
    private readonly TimeSpan _longWorkDuration = TimeSpan.FromMinutes(50);
    private readonly TimeSpan _shortBreakDuration = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _longBreakDuration = TimeSpan.FromMinutes(15);
    private const int _pomodorosBeforeLongBreak = 4;
    
    private Timer? _timer;
    private DateTime _endTime;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimeDisplay), nameof(Progress))]
    private TimeSpan _timeRemaining;
    
    [ObservableProperty]
    private int _completedPomodoros;
    
    [ObservableProperty]
    private bool _isRunning;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsWorkState), nameof(IsBreakState), nameof(StateDisplayName), 
        nameof(IsShortWork), nameof(IsLongWork), nameof(IsShortBreak), nameof(IsLongBreak))]
    private TimerState _currentState;
    
    [ObservableProperty]
    private bool _notificationsEnabled = true;
    
    // Statistics
    [ObservableProperty]
    private TimeSpan _totalWorkTimeToday;
    
    [ObservableProperty]
    private TimeSpan _totalBreakTimeToday;
    
    [ObservableProperty]
    private int _currentStreak;
    
    [ObservableProperty]
    private int _longestStreak;
    
    [ObservableProperty]
    private DateTime _lastSessionDate;
    
    private DateTime _sessionStartTime;
    private DateTime _todayDate;
    
    public TimerViewModel()
    {
        _todayDate = DateTime.Today;
        _lastSessionDate = DateTime.MinValue;
        LoadTodayStatistics();
        SwitchToWork();
    }
    
    public string TimeDisplay => $"{TimeRemaining.Minutes:D2}:{TimeRemaining.Seconds:D2}";
    
    public string StateDisplayName => CurrentState switch
    {
        TimerState.Work => "Short Work",
        TimerState.LongWork => "Long Work",
        TimerState.ShortBreak => "Short Break",
        TimerState.LongBreak => "Long Break",
        _ => "Unknown"
    };
    
    public double Progress
    {
        get
        {
            var totalDuration = CurrentState switch
            {
                TimerState.Work => _workDuration,
                TimerState.LongWork => _longWorkDuration,
                TimerState.ShortBreak => _shortBreakDuration,
                TimerState.LongBreak => _longBreakDuration,
                _ => _workDuration
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
        TimeRemaining = _workDuration;
    }
    
    [RelayCommand]
    private void SwitchToLongWork()
    {
        StopTimer();
        CurrentState = TimerState.LongWork;
        TimeRemaining = _longWorkDuration;
    }
    
    [RelayCommand]
    private void SwitchToShortBreak()
    {
        StopTimer();
        CurrentState = TimerState.ShortBreak;
        TimeRemaining = _shortBreakDuration;
    }
    
    [RelayCommand]
    private void SwitchToLongBreak()
    {
        StopTimer();
        CurrentState = TimerState.LongBreak;
        TimeRemaining = _longBreakDuration;
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
            TimerState.Work => _workDuration,
            TimerState.LongWork => _longWorkDuration,
            TimerState.ShortBreak => _shortBreakDuration,
            TimerState.LongBreak => _longBreakDuration,
            _ => _workDuration
        };
    }
    
    [RelayCommand]
    private void ResetTimer()
    {
        StopTimer();
        TimeRemaining = CurrentState switch
        {
            TimerState.Work => _workDuration,
            TimerState.LongWork => _longWorkDuration,
            TimerState.ShortBreak => _shortBreakDuration,
            TimerState.LongBreak => _longBreakDuration,
            _ => _workDuration
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
            if (CompletedPomodoros % _pomodorosBeforeLongBreak == 0 && CurrentState == TimerState.Work)
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
                }
                
                // Auto-start break after work
                if (CompletedPomodoros % _pomodorosBeforeLongBreak == 0 && CurrentState == TimerState.Work)
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
            // Try to find the sound file in the source directory (for development)
            var soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "alarm.mp3");
            
            if (!System.IO.File.Exists(soundPath))
            {
                // Try looking in the source directory
                var sourceDir = System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                if (sourceDir != null)
                {
                    soundPath = System.IO.Path.Combine(sourceDir, "Assets", "alarm.mp3");
                }
            }
            
            if (!System.IO.File.Exists(soundPath))
            {
                Console.WriteLine($"Sound file not found at: {soundPath}");
                return null;
            }
            
            Console.WriteLine($"Playing sound from: {soundPath}");
            
            // Use ffplay (part of ffmpeg) to play the sound on Linux with loop
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffplay",
                    Arguments = $"-nodisp -loop 0 -volume 50 \"{soundPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            return process;
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
    
    public string TotalWorkTimeTodayDisplay => 
        $"{(int)TotalWorkTimeToday.TotalHours:D2}:{TotalWorkTimeToday.Minutes:D2}:{TotalWorkTimeToday.Seconds:D2}";
    
    public string TotalBreakTimeTodayDisplay => 
        $"{(int)TotalBreakTimeToday.TotalHours:D2}:{TotalBreakTimeToday.Minutes:D2}:{TotalBreakTimeToday.Seconds:D2}";
    
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