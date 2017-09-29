using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FMS.Utils
{
    public static class ConvertHelper
    {

        public static decimal ConvertToDecimalOrZero(string value)
        {
            try
            {
                return GetDecimal(value);
               }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool IsNumeric(string value)
        {
            try
            {
                var result = GetDecimal(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static decimal GetDecimal(string value)
        {
            return Decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public static decimal GetDecimal(int value)
        {
            return GetDecimal(value.ToString());
        }
        public static DateTime? StringToDateTimeCk5FileDocuments(string value)
        {
            if (!IsNumeric(value))
                return null;

            //format ddMMyyyy
            if (value.Length != 8)
                return null;

            try
            {
                return new DateTime(Convert.ToInt32(value.Substring(4, 4)),
                    Convert.ToInt32(value.Substring(2, 2)),
                    Convert.ToInt32(value.Substring(0, 2)));

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsEnumsObject<T>(Object enumsType, int enumsValue)
        {
            //T one = (T)Enum.Parse(typeof(T), o.ToString());
            //return one;
            if (typeof(T).IsEnumDefined(enumsValue))
                return true;
            return false;
        }

        public static string ConvertDecimalToString(decimal? value, string formatDecimalValue = "f2")
        {
            return value.HasValue ? value.Value.ToString(formatDecimalValue) : string.Empty;
        }

        public static string ConvertDateToString(DateTime? value, string formatDate)
        {
            return value.HasValue ? value.Value.ToString(formatDate) : string.Empty;
        }

        public static string ConvertDateToStringddMMMyyyy(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd MMM yyyy") : string.Empty;
        }

        public static string ConvertDecimalToStringMoneyFormat(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("#,##0.#0") : string.Empty;
        }

        public static string ConvertDateToStringHHmm(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("HH:mm") : string.Empty;
        }

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static int? ConvertToInt32OrNull(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string ConvertDecimalFiveToString(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("#,##0.####0") : string.Empty;
        }

        public static string ConvertInt32ToString(int? value)
        {
            try
            {
                if (value.HasValue)
                    return value.ToString();

                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetNumeralFromRomanValue(string roman)
        {
            if (roman == null)
                return null;
            if (roman == "/")
                return null;
            string romanValue = null;

            switch (roman)
            {
                case "0":
                    romanValue = "00";
                    break;

                case "I":
                    romanValue = "01";
                    break;

                case "II":
                    romanValue = "02";
                    break;

                case "III":
                    romanValue = "03";
                    break;

                case "IV":
                    romanValue = "04";
                    break;
                case "V":
                    romanValue = "05";
                    break;
                case "VI":
                    romanValue = "06";
                    break;


                case "VII":
                    romanValue = "07";
                    break;

                case "VIII":
                    romanValue = "08";
                    break;

                case "IX":
                    romanValue = "09";
                    break;

                case "X":
                    romanValue = "10";
                    break;



            }
            return romanValue;


        }

    }
}
