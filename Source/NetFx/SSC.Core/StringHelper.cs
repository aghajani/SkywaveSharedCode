using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace SSC
{
    public class StringHelper
    {
        private static Dictionary<string, string> _FixDigitsMap = new Dictionary<string, string>
        {
            {"٠١٢٣٤٥٦٧٨٩","0123456789"},
            {"۰۱۲۳۴۵۶۷۸۹","0123456789"}
        };
        public static string FixDigits(string s1)
        {
            if (string.IsNullOrWhiteSpace(s1))
                return s1;
            foreach (var fe1 in _FixDigitsMap)
            {
                if (fe1.Key.Length != fe1.Value.Length)
                    continue;
                for (int i1 = 0; i1 < fe1.Key.Length; i1++)
                {
                    s1 = s1.Replace(fe1.Key[i1], fe1.Value[i1]);
                }
            }
            return s1;
        }
        public static string FixMobileNumber(string mobileNo, string countryCode) { return MobileToMSISDN(mobileNo, countryCode); }
        public static string MobileToMSISDN(string mobileNo, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
                return mobileNo;
            mobileNo = FixDigits(mobileNo);
            mobileNo = mobileNo.TrimStart(new char[] { '0', '+', ' ' });
            mobileNo = mobileNo.Trim();
            if (!mobileNo.StartsWith(countryCode))
                mobileNo = countryCode + mobileNo;
            return mobileNo;
        }
        public static bool IsAllDigit(string s1)
        {
            long test1;
            return long.TryParse(s1, out test1);
        }

        bool _IsValidEmail_InvalidDomain = false;
        public bool IsValidEmail(string strIn)
        {
            _IsValidEmail_InvalidDomain = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this._IsValidEmail_DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (_IsValidEmail_InvalidDomain)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string _IsValidEmail_DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _IsValidEmail_InvalidDomain = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public static string MobileRawNumber(string mobileNo, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
                return mobileNo;
            mobileNo = FixDigits(mobileNo);
            mobileNo = mobileNo.TrimStart(new char[] { '0', '+', ' ' });
            mobileNo = mobileNo.Trim();
            if (mobileNo.StartsWith(countryCode))
                mobileNo = mobileNo.Substring(countryCode.Length);
            return mobileNo;
        }

        private static void _CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
        public static string Compress(string toCompress, CompressionLevel level = CompressionLevel.Optimal)
        {
            var bytes = Encoding.UTF8.GetBytes(toCompress);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, level))
                {
                    _CopyTo(msi, gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }
        public static string Decompress(string toDecompress)
        {
            var bytes = Convert.FromBase64String(toDecompress);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    _CopyTo(gs, mso);
                }
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
