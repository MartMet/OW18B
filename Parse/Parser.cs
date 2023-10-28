using OW18B.Enums;
using System.Text;

namespace OW18B.Parse
{
    public class Parser
    {
        internal static void Parse(byte[] input, out short value, out Divisor divisor,out Mode mode, out Prefixes prefixes)
        {
            short twoBytes = BitConverter.ToInt16(new byte[2] { (byte)input[0], (byte)input[1] }, 0);
            prefixes = (Prefixes)((twoBytes & 56) >> 3);
            mode = (Mode)((twoBytes & 960) >> 6);
            divisor = (Divisor)((twoBytes & 7));
            short status = BitConverter.ToInt16(new byte[2] { (byte)input[2], (byte)input[3] }, 0);
            value = BitConverter.ToInt16(new byte[2] { (byte)input[4], (byte)input[5] }, 0);
        }

        internal static string BuildMeasureValue(short valueShort, Divisor divisor, Mode mode, Prefixes prefixes)
        {
            double value = valueShort;
            StringBuilder sb2 = new StringBuilder();
            switch (divisor)
            {
                case Divisor.D1:
                    sb2.Append($" {value}");
                    BuildUnitWithPrefix(sb2, mode, prefixes);
                    break;
                case Divisor.D10:
                    value /= 10;
                    sb2.Append($" {value}");
                    BuildUnitWithPrefix(sb2, mode, prefixes);
                    break;
                case Divisor.D100:
                    value /= 100;
                    sb2.Append($" {value}");
                    BuildUnitWithPrefix(sb2, mode, prefixes);
                    break;
                case Divisor.D1000:
                    value /= 1000;
                    sb2.Append($" {value}");
                    BuildUnitWithPrefix(sb2, mode, prefixes);
                    break;
                case Divisor.D10000:
                    value /= 10000;
                    sb2.Append($" {value}");
                    BuildUnitWithPrefix(sb2, mode, prefixes);
                    break;
                case Divisor.ERR_POINT:
                    sb2.Append("ERR_POINT");
                    break;
                case Divisor.UL:
                    sb2.Append("UL");
                    break;
                case Divisor.OL:
                    sb2.Append("OL");
                    break;
            }
            return sb2.ToString();
        }

            internal static void BuildUnitWithPrefix(StringBuilder sb2, Mode mode, Prefixes prefixes)
        {
            switch (prefixes)
            {
                case Prefixes.Pico:
                    sb2.Append(" p");
                    break;
                case Prefixes.Nano:
                    sb2.Append(" n");
                    break;
                case Prefixes.Micro:
                    sb2.Append(" µ");
                    break;
                case Prefixes.Milli:
                    sb2.Append(" m");
                    break;
                case Prefixes.None:
                    sb2.Append(" ");
                    break;
                case Prefixes.Kilo:
                    sb2.Append(" k");
                    break;
                case Prefixes.Mega:
                    sb2.Append(" M");
                    break;
                case Prefixes.Giga:
                    sb2.Append(" G");
                    break;
            }
            switch (mode)
            {
                case Mode.DC_Voltage:
                    sb2.Append("V");
                    break;
                case Mode.AC_Voltage:
                    sb2.Append("V");
                    break;
                case Mode.DC_Ampere:
                    sb2.Append("A");
                    break;
                case Mode.AC_Ampere:
                    sb2.Append("A");
                    break;
                case Mode.Ohm:
                    sb2.Append("Ω");
                    break;
                case Mode.Farad:
                    sb2.Append("F");
                    break;
                case Mode.Hz:
                    sb2.Append("Hz");
                    break;
                case Mode.Percent:
                    sb2.Append("%");
                    break;
                case Mode.Centigrade:
                    sb2.Append("°C");
                    break;
                case Mode.Fahrenheit:
                    sb2.Append("°F");
                    break;
                case Mode.Diode:
                    sb2.Append("V");
                    break;
                case Mode.Continuity:
                    sb2.Append("Ω");
                    break;
                case Mode.HFEC:
                    break;
                case Mode.NCV:
                    break;
            }
        }

        internal static string BuildMeasureMode(Mode mode)
        {
            StringBuilder sb = new StringBuilder();

            switch (mode)
            {
                case Mode.DC_Voltage:
                    sb.Append("VDC");
                    break;
                case Mode.AC_Voltage:
                    sb.Append("VAC");
                    break;
                case Mode.DC_Ampere:
                    sb.Append("DC");
                    break;
                case Mode.AC_Ampere:
                    sb.Append("AC");
                    break;
                case Mode.Ohm:
                    sb.Append("Ohm");
                    break;
                case Mode.Farad:
                    sb.Append("Capacity");
                    break;
                case Mode.Hz:
                    sb.Append("Frequency");
                    break;
                case Mode.Percent:
                    sb.Append("Percent");
                    break;
                case Mode.Centigrade:
                    sb.Append("Centigrade");
                    break;
                case Mode.Fahrenheit:
                    sb.Append("Fahrenheit");
                    break;
                case Mode.Diode:
                    sb.Append("Diode");
                    break;
                case Mode.Continuity:
                    sb.Append("Continuity");
                    break;
                case Mode.HFEC:
                    sb.Append("HFEC");
                    break;
                case Mode.NCV:
                    sb.Append("NCV");
                    break;
            }
            return sb.ToString();
        }
    }
}
