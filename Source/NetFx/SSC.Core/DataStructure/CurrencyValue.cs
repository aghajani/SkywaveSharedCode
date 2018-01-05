using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SSC.DataStructure
{
    [DataContract]
    public class CurrencyValue : IComparable
    {
        public CurrencyValue()
        {
        }
        public CurrencyValue(double? value)
            : this(value, 0, "fa-IR")
        { }
        public CurrencyValue(double? value, string cultureInfoName)
            : this(value, 0, cultureInfoName)
        { }
        public CurrencyValue(double? value, System.Globalization.CultureInfo cultureInfo)
            : this(value, 0, cultureInfo)
        { }
        public CurrencyValue(double? value, int decimalCount, System.Globalization.CultureInfo cultureInfo)
            : this(value, decimalCount, (cultureInfo == null) ? "" : cultureInfo.Name)
        { }
        public CurrencyValue(double? value, int? decimalCount, string cultureInfoName)
        {
            Value = value;
            Display_DecimalCount = (decimalCount == null) ? 0 : decimalCount.Value;
            Display_CultureInfo_Name = cultureInfoName;
        }

        public static CurrencyValue operator +(CurrencyValue v1, CurrencyValue v2)
        {
            if (v1.Value != null && v2.Value != null)
                return new CurrencyValue(v1.Value.Value + v2.Value.Value);
            else
                return new CurrencyValue(null);
        }
        public static CurrencyValue operator +(CurrencyValue v1, double v2)
        {
            if (v1.Value != null)
                return new CurrencyValue(v1.Value.Value + v2);
            else
                return new CurrencyValue(null);
        }
        public static CurrencyValue operator +(double v1, CurrencyValue v2)
        {
            if (v2.Value != null)
                return new CurrencyValue(v2.Value.Value + v1);
            else
                return new CurrencyValue(null);
        }
        public static CurrencyValue operator -(CurrencyValue v1, CurrencyValue v2)
        {
            if (v1.Value != null && v2.Value != null)
                return new CurrencyValue(v1.Value.Value - v2.Value.Value);
            else
                return new CurrencyValue(null);
        }
        public static CurrencyValue operator -(CurrencyValue v1, double v2)
        {
            if (v1.Value != null)
                return new CurrencyValue(v1.Value.Value - v2);
            else
                return new CurrencyValue(null);
        }
        public static CurrencyValue operator -(double v1, CurrencyValue v2)
        {
            if (v2.Value != null)
                return new CurrencyValue(v1 - v2.Value.Value);
            else
                return new CurrencyValue(null);
        }

        [DataMember]
        public double? Value { get; set; }

        [DataMember]
        public string Display_CultureInfo_Name { get; set; }

        [DataMember]
        public int Display_DecimalCount { get; set; } = 0;

        private System.Globalization.CultureInfo _CultureInfo;
        public override string ToString()
        {
            return ToString(Value);
        }
        public string ToString(double? value)
        {
            if (_CultureInfo == null)
            {
                if (string.IsNullOrEmpty(Display_CultureInfo_Name))
                    _CultureInfo = new System.Globalization.CultureInfo("fa-IR");
                else
                    _CultureInfo = new System.Globalization.CultureInfo(Display_CultureInfo_Name);
            }
            return (value == null) ? "" : value.Value.ToString("C" + Display_DecimalCount.ToString(), _CultureInfo);
        }

        public int CompareTo(object obj)
        {
            double? compareTo = null;
            if (obj == null)
                compareTo = null;
            else if (obj is double?)
                compareTo = (double?)obj;
            else if (obj is double)
                compareTo = (double)obj;
            else if (obj is CurrencyValue)
                compareTo = ((CurrencyValue)obj).Value;
            //
            if (Value == null && compareTo == null)
                return 0;
            else if (Value == null && compareTo != null)
                return -1;
            else if (Value != null && compareTo == null)
                return 1;
            else
                return Value.Value.CompareTo(compareTo.Value);
        }
    }
}
