namespace FMS.Utils
{
    public class MonthHelper
    {
        public static string ConvertToRomansNumeral(int monthId)
        {
            string[] arrRomansNumeralMonth = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII" };
            return arrRomansNumeralMonth[monthId - 1];
        }
    }
}
