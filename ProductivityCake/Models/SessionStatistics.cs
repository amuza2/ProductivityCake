using System;

namespace ProductivityCake.Models;

public class SessionStatistics
{
    public DateTime Date { get; set; }
    public TimeSpan TotalWorkTime { get; set; }
    public TimeSpan TotalBreakTime { get; set; }
    public int CompletedPomodoros { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime LastSessionDate { get; set; }
    
    public SessionStatistics()
    {
        Date = DateTime.Today;
        TotalWorkTime = TimeSpan.Zero;
        TotalBreakTime = TimeSpan.Zero;
        CompletedPomodoros = 0;
        CurrentStreak = 0;
        LongestStreak = 0;
        LastSessionDate = DateTime.MinValue;
    }
}
