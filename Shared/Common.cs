using System;
using static Readible.Shared.Const;

namespace Readible.Shared
{
    public class Common
    {
        public static bool CompareTimestamp(DateTime? from, DateTime? to)
        {
            var timespan = from - to;
            if (!timespan.HasValue) return false;
            var value = timespan.Value;

            if (Math.Abs(value.TotalDays) > COMPARE_TOLERANCE) return false;
            if (Math.Abs(value.TotalHours) > COMPARE_TOLERANCE) return false;
            if (Math.Abs(value.TotalMinutes) > COMPARE_TOLERANCE) return false;
            if (Math.Abs(value.TotalSeconds) > COMPARE_TOLERANCE) return false;
            return !(Math.Abs(value.TotalMilliseconds) > COMPARE_TOLERANCE);
        }

        public static int IntParse(string number, int defaultValue = 0)
        {
            try
            {
                return int.Parse(number);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static double DoubleParse(string number, double defaultValue = 0.0)
        {
            try
            {
                return double.Parse(number);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string FillDigit(int number, int length = 2, char chara = '0')
        {
            if (length <= 0) throw new ArgumentOutOfRangeException();

            var numberString = number.ToString();
            if (number >= Math.Pow(10, length - 1)) return numberString;

            var tmpString = new string(chara, length) + numberString;
            return tmpString.Substring(numberString.Length);
        }

        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public static double CalcDiscountPercent(double price, int discount) => discount == 0 ? price : price * (100 - discount) / 100;
    }
}