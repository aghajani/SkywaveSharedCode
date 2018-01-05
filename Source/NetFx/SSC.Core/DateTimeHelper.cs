#if !PCL
using System;
using System.Collections.Generic;
using System.Text;

namespace SSC
{
    /// <summary>
    /// Contains different types of date/time formats supported by ToFormattedString function.
    /// </summary>
    public enum DateTimeHelperFormats
    {
        DayOfWeekAndShortDate = 0,
        DayOfWeekAndMediumDate = 1,
        MediumDate = 2,
        ShortDate = 3,
        Year = 4,
        Month = 5,
        DayOfWeek = 6,
        YearAndMonth = 7,
        MonthName = 8,
        YearAndMonthName = 9
    }
    /// <summary>
    /// Calendars supported by MaDateTime
    /// </summary>
    public enum DateTimeHelperCalendars
    {
        Persian = 0,
        Gregorian = 1
    }
    /// <summary>
    /// This class supports converting date/time from system date/time (gregorian) to different calendar types using simple methods and properties.
    /// </summary>
    public class DateTimeHelper
    {
        private static System.Globalization.PersianCalendar cPersian = new System.Globalization.PersianCalendar();
        private static System.Globalization.GregorianCalendar cGregorian = new System.Globalization.GregorianCalendar(System.Globalization.GregorianCalendarTypes.USEnglish);

        private DateTime? _DateTime;
        /// <summary>
        /// Always returns stored system date/time (gregorian)
        /// </summary>
        public DateTime? DateTime
        {
            get { return _DateTime; }
        }

        private DateTimeHelperCalendars _Calendar;
        /// <summary>
        /// Target calendar (system date/time will be converted to this calendar type)
        /// </summary>
        public DateTimeHelperCalendars Calendar
        {
            get { return _Calendar; }
        }
        /// <summary>
        /// Returns year of converted date (using target calendar type).
        /// </summary>
        public int Year { get; private set; }
        /// <summary>
        /// Returns month of converted date (using target calendar type).
        /// </summary>
        public int Month { get; private set; }
        /// <summary>
        /// Returns day of converted date (using target calendar type).
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// Returns 'day of week' of converted date (using target calendar type).
        /// </summary>
        public DayOfWeek DayOfWeek { get; private set; }

        public DateTimeHelper(DateTimeHelperCalendars calendarTarget, DateTime? sourceDate)
            : this(calendarTarget, sourceDate, null) { }
        public DateTimeHelper(DateTimeHelperCalendars calendarTarget, DateTime? sourceDate, DateTimeKind? targetTimeKind)
        {
            _Calendar = calendarTarget;
            _DateTime = sourceDate;
            if (_DateTime != null && targetTimeKind != null)
                switch (targetTimeKind.Value)
                {
                    case DateTimeKind.Local:
                        _DateTime = _DateTime.Value.ToLocalTime();
                        break;
                    case DateTimeKind.Unspecified:
                        break;
                    case DateTimeKind.Utc:
                        _DateTime = _DateTime.Value.ToUniversalTime();
                        break;
                    default:
                        break;
                }
            InitializeMe();
        }
        public DateTimeHelper(DateTimeHelperCalendars calendarTarget, DateTimeHelperCalendars calendarSource, int year, int month, int day)
            : this(calendarTarget, calendarSource, year, month, day, 0, 0, 0, 0, DateTimeKind.Unspecified) { }
        public DateTimeHelper(DateTimeHelperCalendars calendarTarget, DateTimeHelperCalendars calendarSource, int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            _Calendar = calendarTarget;
            switch (calendarSource)    //Put the amounts into source calender and set it as a default calender
            {
                case DateTimeHelperCalendars.Gregorian:
                    _DateTime = cGregorian.ToDateTime(year, month, day, hour, minute, second, millisecond);
                    _DateTime = System.DateTime.SpecifyKind(_DateTime.Value, kind);
                    //_DateTime = new DateTime(year, month, day, hour, minute, second, millisecond, cGregorian, kind);
                    break;
                case DateTimeHelperCalendars.Persian:
                default:
                    _DateTime = cPersian.ToDateTime(year, month, day, hour, minute, second, millisecond);
                    _DateTime = System.DateTime.SpecifyKind(_DateTime.Value, kind);
                    //_DateTime = new DateTime(year, month, day, hour, minute, second, millisecond, cPersian, kind);
                    break;
            }
            InitializeMe();
        }

        private void InitializeMe()
        {
            System.Globalization.Calendar c1 = GetCalendar(_Calendar);
            if (_DateTime != null && _DateTime.Value.Year > 690)
            {
                Year = c1.GetYear(_DateTime.Value);
                Month = c1.GetMonth(_DateTime.Value);
                Day = c1.GetDayOfMonth(_DateTime.Value);
                DayOfWeek = c1.GetDayOfWeek(_DateTime.Value);
            }
            else
                _DateTime = null;
        }

        /// <summary>
        /// Returns name of 'day of week' of converted date in appropriate language.
        /// </summary>
        /// <returns>Returns name of 'day of week' of converted date in appropriate language.</returns>
        public string GetDayOfWeekName()
        {
            return GetDayOfWeekName(_Calendar, DayOfWeek);
        }

        /// <summary>
        /// Returns name of 'month number' of converted date in appropriate language.
        /// </summary>
        /// <returns>Returns name of 'month number' of converted date in appropriate language.</returns>
        public string GetMonthName()
        {
            return GetMonthName(_Calendar, Month);
        }

        /// <summary>
        /// Returns the converted input date in "year/month/day" format.
        /// </summary>
        /// <returns>Returns the converted input date in "year/month/day" format.</returns>
        public override string ToString() { return ToString("/"); }
        /// <summary>
        /// Returns the converted input date in "year'inTxt'month'inTxt'day" format.
        /// </summary>
        /// <returns>Returns the converted input date in "year'inTxt'month'inTxt'day" format.</returns>
        public string ToString(string inTxt) { return (_DateTime == null) ? "" : string.Format("{0:0000}", Year) + inTxt + string.Format("{0:00}", Month) + inTxt + string.Format("{0:00}", Day); }

        public string ToCustomString(string customFormat)
        {
            if (_DateTime == null)
                return "";
            else
            {
                var r = customFormat;
                r = r.Replace("yyyy", string.Format("{0:0000}", Year));
                r = r.Replace("MM", string.Format("{0:00}", Month));
                r = r.Replace("dd", string.Format("{0:00}", Day));
                r = r.Replace("HH", string.Format("{0:00}", _DateTime.Value.Hour));
                r = r.Replace("mm", string.Format("{0:00}", _DateTime.Value.Minute));
                r = r.Replace("ss", string.Format("{0:00}", _DateTime.Value.Second));
                r = r.Replace("ffff", string.Format("{0:0000}", _DateTime.Value.Millisecond));
                return r;
            }
        }

        /// <summary>
        /// Returns the converted input date in different formats due to the various values of "MaDateTimeFormats" format parameter and optionally attaching time at the end.
        /// </summary>
        /// <returns>Returns the converted input date/time in selected "MaDateTimeFormats" format and optionally attaching time at the end.</returns>
        public string ToFormatedString(DateTimeHelperFormats format)
        {
            return ToFormatedString(format, true);
        }
        public string ToFormatedString(DateTimeHelperFormats format, bool includeTime)
        {
            if (includeTime)
                return ToFormatedString(format, true, "HH:mm:ss");
            else
                return ToFormatedString(format, false, "");
        }
        public string ToFormatedString(DateTimeHelperFormats format, string timeFormat)
        {
            return ToFormatedString(format, true, timeFormat);
        }
        private string ToFormatedString(DateTimeHelperFormats format, bool includeTime, string timeFormat)
        {
            string r = "";
            if (DateTime != null)
            {
                System.DateTime date1 = DateTime.Value;
                //switch (displayKind)
                //{
                //    case DateTimeKind.Utc:
                //        date1 = date1.ToUniversalTime();
                //        break;
                //    case DateTimeKind.Local:
                //    default:
                //        date1 = date1.ToLocalTime();
                //        break;
                //}

                switch (format)
                {
                    case DateTimeHelperFormats.DayOfWeekAndShortDate:
                        r = ToString() + " " + GetDayOfWeekName();
                        break;
                    case DateTimeHelperFormats.DayOfWeekAndMediumDate:
                        r = GetDayOfWeekName() + " " + Day.ToString() + " " + GetMonthName() + " " + Year.ToString();
                        break;
                    case DateTimeHelperFormats.MediumDate:
                        r = Day.ToString() + " " + GetMonthName() + " " + Year.ToString();
                        break;
                    case DateTimeHelperFormats.ShortDate:
                        r = ToString();
                        break;
                    case DateTimeHelperFormats.Year:
                        r = Year.ToString();
                        break;
                    case DateTimeHelperFormats.Month:
                        r = Month.ToString();
                        break;
                    case DateTimeHelperFormats.DayOfWeek:
                        r = GetDayOfWeekName();
                        break;
                    case DateTimeHelperFormats.MonthName:
                        r = GetMonthName();
                        break;
                    case DateTimeHelperFormats.YearAndMonth:
                        r = Year.ToString() + " " + Month.ToString();
                        break;
                    case DateTimeHelperFormats.YearAndMonthName:
                        r = Year.ToString() + " " + GetMonthName();
                        break;
                    default:
                        break;
                }
                if (includeTime)
                {
                    r += " ";
                    if (string.IsNullOrEmpty(timeFormat))
                        r += date1.ToString("t");
                    else
                        r += date1.ToString(timeFormat);
                }
            }
            return r;
        }

        /// <summary>
        /// Gets .Net Calendar equivalence of MaDateTimeCalendars enum.
        /// </summary>
        /// <param name="calendar1">Enum value to get its .Net Calendar equivalence.</param>
        /// <returns>Returns .Net Calendar equivalence of MaDateTimeCalendars enum.</returns>
        public static System.Globalization.Calendar GetCalendar(DateTimeHelperCalendars calendar1)  //return the source calender
        {
            switch (calendar1)
            {
                case DateTimeHelperCalendars.Gregorian:
                    return cGregorian;
                case DateTimeHelperCalendars.Persian:
                default:
                    return cPersian;
            }
        }

        /// <summary>
        /// Deterines if the input date is valid in the target calendar type.
        /// </summary>
        /// <param name="calendar1">Target calendar to check input date in that calendar.</param>
        /// <param name="year">Year of input date to check.</param>
        /// <param name="month">Month of input date to check.</param>
        /// <param name="day">Day of input date to check.</param>
        /// <returns>True if valid, false if not valid.</returns>
        public static bool IsValidDate(DateTimeHelperCalendars calendar1, int year, int month, int day)   //check if the date time is invalid
        {
            System.Globalization.Calendar c1 = GetCalendar(calendar1);
            bool r = false;
            int temp1;
            if (year <= c1.MaxSupportedDateTime.Year && year >= c1.MinSupportedDateTime.Year)
            {
                temp1 = c1.GetMonthsInYear(year);
                if (month >= 1 && month <= temp1)
                {
                    temp1 = c1.GetDaysInMonth(year, month);
                    if (day >= 1 && day <= temp1)
                    {
                        r = true;
                    }
                }
            }
            return r;
        }

        public static bool Equals(DateTimeHelper Date1, DateTimeHelper Date2)
        {
            return Date1.DateTime.Equals(Date2.DateTime);
        }

        /// <summary>
        /// Exchanges the input 'day of the week' into alphabetical name using target calendar.
        /// </summary>
        /// <param name="calendar">Target calendar.</param>
        /// <param name="dayOfWeek">'day of week' to exchange.</param>
        /// <returns>Exchanged alphabetical name using target calendar.</returns>
        public static string GetDayOfWeekName(DateTimeHelperCalendars calendar, DayOfWeek dayOfWeek)
        {
            switch (calendar)
            {
                case DateTimeHelperCalendars.Gregorian:
                    return dayOfWeek.ToString();
                case DateTimeHelperCalendars.Persian:
                default:
                    switch (dayOfWeek)
                    {
                        case System.DayOfWeek.Friday:
                            return "جمعه";
                        case System.DayOfWeek.Monday:
                            return "دوشنبه";
                        case System.DayOfWeek.Saturday:
                            return "شنبه";
                        case System.DayOfWeek.Sunday:
                            return "یكشنبه";
                        case System.DayOfWeek.Thursday:
                            return "پنجشنبه";
                        case System.DayOfWeek.Tuesday:
                            return "سه‌شنبه";
                        case System.DayOfWeek.Wednesday:
                            return "چهارشنبه";
                        default:
                            return "";
                    }
            }
        }

        /// <summary>
        /// Exchanges the input month number into alphabetical name using target calendar.
        /// </summary>
        /// <param name="calendar">Target calendar.</param>
        /// <param name="month">Month number to exchange.</param>
        /// <returns>Exchanged alphabetical name using target calendar.</returns>
        public static string GetMonthName(DateTimeHelperCalendars calendar, int month)    //exchange the month number into alphabetical name
        {
            switch (calendar)
            {
                case DateTimeHelperCalendars.Gregorian:
                    switch (month)
                    {
                        case 1:
                            return "Janury";
                        case 2:
                            return "February";
                        case 3:
                            return "March";
                        case 4:
                            return "April";
                        case 5:
                            return "May";
                        case 6:
                            return "June";
                        case 7:
                            return "July";
                        case 8:
                            return "August";
                        case 9:
                            return "September";
                        case 10:
                            return "October";
                        case 11:
                            return "November";
                        case 12:
                            return "December";
                        default:
                            return "Out of bound";
                    }
                case DateTimeHelperCalendars.Persian:
                default:
                    switch (month)
                    {
                        case 1:
                            return "فروردین";
                        case 2:
                            return "اردیبهشت";
                        case 3:
                            return "خرداد";
                        case 4:
                            return "تیر";
                        case 5:
                            return "مرداد";
                        case 6:
                            return "شهریور";
                        case 7:
                            return "مهر";
                        case 8:
                            return "آبان";
                        case 9:
                            return "آذر";
                        case 10:
                            return "دی";
                        case 11:
                            return "بهمن";
                        case 12:
                            return "اسفند";
                        default:
                            return "خارج از محدوده";
                    }
            }
        }

        public static DateTimeHelper Parse(string input, DateTimeHelperCalendars calendarSource = DateTimeHelperCalendars.Persian, DateTimeHelperCalendars calendarTarget = DateTimeHelperCalendars.Gregorian, char seperator = '/')
        {
            DateTimeHelper r = null;
            //
            string[] parts = input.Split(new char[] { seperator });
            if (parts.Length == 3)
            {
                int y, m, d;
                try
                {
                    y = int.Parse(parts[0]);
                    m = int.Parse(parts[1]);
                    d = int.Parse(parts[2]);
                    if (IsValidDate(calendarSource, y, m, d))
                        r = new DateTimeHelper(calendarTarget, calendarSource, y, m, d);
                }
                catch (Exception)
                {
                }
            }
            //
            return r;
        }
    }
}
#endif