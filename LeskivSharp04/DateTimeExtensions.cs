using System;

namespace LeskivSharp02
{
    public static class DateTimeExtensions
    {
        public static int YearsPassedCnt(this DateTime thisDateTime, DateTime anotherDateTime)
        {
            return (thisDateTime.Year - anotherDateTime.Year) - (thisDateTime.DayOfYear >= anotherDateTime.DayOfYear ? 0 : 1);
        }
    }
}
