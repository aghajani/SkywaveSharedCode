using System;
using System.Collections.Generic;
using System.Text;

namespace SSC
{
    public static class TimeSpanHelper
    {
        public static string AsAdjective(this TimeSpan inTimeSpan, System.Resources.ResourceManager rm)
        {
            var r = "";
            var daysInMonth = 30;
            var monthesInYear = 12;
            if (inTimeSpan.TotalHours < 24)
                r = string.Format(rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Hours)), (int)inTimeSpan.TotalHours);
            else if (inTimeSpan.TotalDays == 1)
                r = rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Day_1));
            else if (inTimeSpan.TotalDays == 7)
                r = rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Week_1));
            else if (inTimeSpan.TotalDays < daysInMonth)
                r = string.Format(rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Days)), (int)inTimeSpan.TotalDays);
            else if (inTimeSpan.TotalDays == daysInMonth)
                r = rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Month_1));
            else if (inTimeSpan.TotalDays / daysInMonth < monthesInYear)
                r = string.Format(rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Monthes)), (int)Math.Floor(inTimeSpan.TotalDays / daysInMonth));
            else if ((int)(inTimeSpan.TotalDays / daysInMonth) == monthesInYear)
                r = rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Year_1));
            else
                r = string.Format(rm.GetString(nameof(ResourceManagerStrings.SSC_Time_Years)), (int)Math.Floor(inTimeSpan.TotalDays / daysInMonth));
            return r;
        }
    }
}
