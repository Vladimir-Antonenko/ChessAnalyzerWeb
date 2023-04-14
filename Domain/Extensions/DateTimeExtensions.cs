namespace Domain.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime value)
    {
        return (long)(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }

    public static long ToUnixTimestamp(this DateTime? value)
    {
        return (value ?? default).ToUnixTimestamp();
    }

    public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
    {
        return dateToCheck >= startDate && dateToCheck <= endDate;
    }
}