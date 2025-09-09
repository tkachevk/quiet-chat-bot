namespace QuietChatBot.Helpers;

using TimeZoneConverter;

public class TimeHelper
{
    private readonly string _timeZoneId;

    public TimeHelper(string timeZoneId)
    {
        _timeZoneId = timeZoneId;
    }

    public DateTime GetEndOfDayUtc()
    {
        var tz = TZConvert.GetTimeZoneInfo(_timeZoneId);

        var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
        var endOfDayLocal = nowLocal.Date.AddDays(1);

        return TimeZoneInfo.ConvertTimeToUtc(endOfDayLocal, tz);
    }
}