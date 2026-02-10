using System.Globalization;

namespace SolarLiveStatusPanel
{
    internal class Utils
    {
        public static string FormatKw(decimal? w)
        {
            if (w == null)
            {
                return "not present";
            }
            decimal watts = w.GetValueOrDefault();
            if (Math.Abs(watts) > 1000)
            {
                return $"{(watts / 1000).ToString("0.00")} KW";
            }
            if (watts != 0)
            {
                return $"{(watts).ToString("0.00")} W";
            }
            return "0 W";
        }

        public static string FormatAmps(decimal? a)
        {
            if (a == null)
            {
                return "not present";
            }
            decimal amps = a.GetValueOrDefault();
            if (amps != 0)
            {
                return $"{(amps).ToString("0")} A";
            }
            return "0 A";
        }

        public static string FormatVolts(decimal? v)
        {
            if (v == null)
            {
                return "not present";
            }
            decimal volts = v.GetValueOrDefault();
            if (volts != 0)
            {
                return $"{(volts).ToString("0.0")} V";
            }
            return "0 V";
        }

        static public string FormatSeconds(string? seconds)
        {
            long sec = 0;
            Int64.TryParse(seconds, out sec);
            return FormatSeconds(sec);
        }

        static public string FormatSeconds(decimal? seconds)
        {
            if (seconds == null)
            {
                return "not present";
            }
            if (seconds < 0)
            {
                return "less than zero seconds";
            }
            long sec = decimal.ToInt64(seconds.GetValueOrDefault());
            return FormatSeconds(sec);
        }

        static public string FormatSeconds(long? s)
        {
            if (s == null)
            {
                return "not present";
            }

            long seconds = s.GetValueOrDefault();

            long minutes = 0;
            double hours = 0;
            double days = 0;

            if (seconds > 0)
            {
                minutes = (seconds / 60);
            }
            if (minutes > 0)
            {
                hours = (minutes / 60);
            }
            if (hours > 0)
            {
                days = (hours / 24);
            }

            if (days > 1)
            {
                return $"{days.ToString("0")} days";
            }
            if (hours > 0)
            {
                var minsInHour = minutes - hours * 60;
                return $"{hours.ToString("0")}:{minsInHour.ToString("00")}";
            }
            if (minutes > 1)
            {
                return $"{minutes.ToString("0")} minutes";
            }
            return $"{seconds.ToString("0")} seconds";
        }
    }
}
