using System;
using System.Collections.Generic;

namespace ProductivityCake.Models;

public class HeatmapDay
{
    public DateTime Date { get; set; }
    public int SessionCount { get; set; }
    public string Color { get; set; } = "#374151";
    public string Tooltip { get; set; } = "";
}

public class HeatmapWeek
{
    public List<HeatmapDay> Days { get; set; } = new();
}

public class HeatmapMonth
{
    public string MonthName { get; set; } = "";
    public double Width { get; set; }
}
