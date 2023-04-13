using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;

namespace Domain.Common;

public class DateRange
{
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    //private DateRange(DateOnly start, DateOnly end)
    //{
    //    Start = start;
    //    End = end;
    //}

    private DateRange(DateTime? start, DateTime? end) 
    {
        Start = DateOnly.FromDateTime(start ?? DateTime.MinValue);
        End = DateOnly.FromDateTime(end ?? DateTime.MaxValue);
    }

    public bool InRange(string? yearMonth)
    {
        DateOnly.TryParseExact(yearMonth, "yyyy/MM", out var date);      
        return Start <= date && End >= date;
    }

   // public static DateRange Create(DateOnly start, DateOnly end) => new DateRange(start, end);
    public static DateRange Create(DateTime? start, DateTime? end) => new DateRange(start, end);
}