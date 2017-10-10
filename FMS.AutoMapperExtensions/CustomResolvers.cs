using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;


namespace FMS.AutoMapperExtensions
{
    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToDateResolver : ValueResolver<object, DateTime?>
    {
        protected override DateTime? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return DateTime.Parse(InputAsString, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Resolve nullable DateTime to a String as CultureInfo.InvariantCulture
    /// </summary>
    public class DateToStringResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Resolve decimal to a int
    /// </summary>
    public class DecimalToIntResolver : ValueResolver<object, int>
    {
        protected override int ResolveCore(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception)
            {

                return -1;
            }

        }
    }

    /// <summary>
    /// Resolve int to a decimal
    /// </summary>
    public class IntToDecimalResolver : ValueResolver<object, decimal>
    {
        protected override decimal ResolveCore(object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception)
            {

                return -1;
            }

        }
    }

    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToNullableIntegerResolver : ValueResolver<object, int?>
    {
        protected override int? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return int.Parse(InputAsString);
        }
    }

    public class NullableBooleanToStringDeletedResolver : ValueResolver<bool?, string>
    {
        protected override string ResolveCore(bool? value)
        {
            if (!value.HasValue)
                return "No";
            return value.Value ? "Yes" : "No";
        }
    }

    public class StringToBooleanResolver : ValueResolver<string, bool?>
    {
        protected override bool? ResolveCore(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return true;
            }

            if (source == "Yes")
            {
                return true;
            }
            return false;
        }
    }

    

    public class StringToDecimalResolver : ValueResolver<string, decimal>
    {
        protected override decimal ResolveCore(string value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

                return -1;
            }

        }
    }

    public class DecimalToStringResolver : ValueResolver<decimal?, string>
    {
        protected override string ResolveCore(decimal? value)
        {
            if (!value.HasValue)
                return "0";

            return value.Value.ToString();

        }

    }
    public class DecimalToStringMoneyResolver : ValueResolver<decimal?, string>
    {
        protected override string ResolveCore(decimal? value)
        {
            if (!value.HasValue)
                return "0";

            return value.Value.ToString("#,##0.#0");

        }

    }

    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToNullableDecimalResolver : ValueResolver<object, decimal?>
    {
        protected override decimal? ResolveCore(object value)
        {
            string InputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(InputAsString))
                return null;

            return decimal.Parse(InputAsString);
        }
    }

    

    public class ConcatStringResolver : ValueResolver<string, string>
    {
        protected override string ResolveCore(string value)
        {
            try
            {
                if (value.Length > 30)
                    return value.Substring(0, 30) + "...";

                return value;
            }
            catch (Exception ex)
            {

                return value;
            }

        }
    }

    public class StringToIntResolver : ValueResolver<string, int>
    {
        protected override int ResolveCore(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {

                return -1;
            }

        }
    }

    public class DoubleToStringMoneyResolver : ValueResolver<double?, string>
    {
        protected override string ResolveCore(double? value)
        {
            if (!value.HasValue)
                return "0";

            return ((double)value).ToString("n2", CultureInfo.InvariantCulture);

        }

    }
    public class DateToStringDDMMMYYYYResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((DateTime)value).ToString("dd-MMM-yyyy");
        }
    }

    public class DecimalToStringMoneyResolver2 : ValueResolver<decimal?, string>
    {
        protected override string ResolveCore(decimal? value)
        {
            if (!value.HasValue)
                return "0.00";

            return ((decimal)value).ToString("n2", CultureInfo.InvariantCulture);

        }

    }
}
