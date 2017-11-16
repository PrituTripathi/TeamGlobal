using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TeamGlobal.Web.Helper
{
    public static class DateTimeHelper
    {
        static Dictionary<string, string> _timeZones = new Dictionary<string, string>() {
            {"ACDT", "+10:30"},
            {"ACST", "+09:30"},
            {"ADT", "-03:00"},
            {"AEDT", "+11:00"},
            {"AEST", "+10:00"},
            {"AHDT", "-09:00"},
            {"AHST", "-10:00"},
            {"AST", "-04:00"},
            {"AT", "-02:00"},
            {"AWDT", "+09:00"},
            {"AWST", "+08:00"},
            {"BAT", "+03:00"},
            {"BDST", "+02:00"},
            {"BET", "-11:00"},
            {"BST", "-03:00"},
            {"BT", "+03:00"},
            {"BZT2", "-03:00"},
            {"CADT", "+10;30"},
            {"CAST", "+09:30"},
            {"CAT", "-10:00"},
            {"CCT", "+08:00"},
            {"CDT", "-05:00"},
            {"CED", "+02:00"},
            {"CET", "+01:00"},
            {"CEST", "+02:00"},
            {"CST", "-06:00"},
            {"EAST", "+10:00"},
            {"EDT", "-04:00"},
            {"EED", "+03:00"},
            {"EET", "+02:00"},
            {"EEST", "+03:00"},
            {"EST", "-05:00"},
            {"FST", "+02:00"},
            {"FWT", "+01:00"},
            {"GMT", "GMT"},
            {"GST", "+10:00"},
            {"HDT", "-09:00"},
            {"HST", "-10:00"},
            {"IDLE", "+12:00"},
            {"IDLW", "-12:00"},
            {"IST", "+05:30"},
            {"IT", "+03:30"},
            {"JST", "+09:00"},
            {"JT", "+07:00"},
            {"MDT", "-06:00"},
            {"MED", "+02:00"},
            {"MET", "+01:00"},
            {"MEST", "+02:00"},
            {"MEWT", "+01:00"},
            {"MST", "-07:00"},
            {"MT", "+08:00"},
            {"NDT", "-02:30"},
            {"NFT", "-03:30"},
            {"NT", "-11:00"},
            {"NST", "+06:30"},
            {"NZ", "+11:00"},
            {"NZST", "+12:00"},
            {"NZDT", "+13:00"},
            {"NZT", "+12:00"},
            {"PDT", "-07:00"},
            {"PST", "-08:00"},
            {"ROK", "+09:00"},
            {"SAD", "+10:00"},
            {"SAST", "+09:00"},
            {"SAT", "+09:00"},
            {"SDT", "+10:00"},
            {"SST", "+02:00"},
            {"SWT", "+01:00"},
            {"USZ3", "+04:00"},
            {"USZ4", "+05:00"},
            {"USZ5", "+06:00"},
            {"USZ6", "+07:00"},
            {"UT", "-00:00"},
            {"UTC", "-00:00"},
            {"UZ10", "+11:00"},
            {"WAT", "-01;00"},
            {"WET", "-00;00"},
            {"WST", "+08:00"},
            {"YDT", "-08:00"},
            {"YST", "-09:00"},
            {"ZP4", "+04;00"},
            {"ZP5", "+05:00"},
            {"ZP6", "+06:00"}
        };


        public static string GetDateTimeFormateWithIST(this string dateTimeString)
        {
            try
            {
                string utc = dateTimeString.Substring(20, dateTimeString.Length - 20);

                int year = Convert.ToInt32(dateTimeString.Substring(0, 4));
                int month = Convert.ToInt32(dateTimeString.Substring(5, 2));
                int day = Convert.ToInt32(dateTimeString.Substring(8, 2));



                int hour = Convert.ToInt32(dateTimeString.Substring(11, 2));
                int minute = Convert.ToInt32(dateTimeString.Substring(14, 2));
                int second = Convert.ToInt32(dateTimeString.Substring(17, 2));

                DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
                string result = String.Format("{0:ddd d}{1} {0:MMM yyyy / HH:mm:ss} {2}", dateTime,
                       (DateTime.Now.Day % 10 == 1 && DateTime.Now.Day != 11) ? "st"
                       : (DateTime.Now.Day % 10 == 2 && DateTime.Now.Day != 12) ? "nd"
                       : (DateTime.Now.Day % 10 == 3 && DateTime.Now.Day != 13) ? "rd"
                       : "th", utc);
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                return dateTimeString;
            }
        }

        public static string ToOrdinal(this DateTime value)
        {
            return value.ToString("ddd") + " "
              + value.Day + value.Day.ToOrdinal() + " " +
              value.ToString("MMM") + ", "
              + value.Year;
        }


        public static string ToOrdinalWithTime(this DateTime value)
        {
            return DateTime.Now.ToString("r");
        }

        public static string ToOrdinal(this int value)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int last_digits = value % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (last_digits < 11 || last_digits > 13)
            {
                // Check the last digit.
                switch (last_digits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return extension;
        }
    }
}