using OW18B.Enums;

namespace OW18B.DataManagement
{
    public class Measurement
    {
        public Divisor Divisor { get; set; }
        public Mode Mode { get; set; }
        public Prefixes Prefixes { get; set; }
        public short Value { get; set; }
        public DateTime DateTime { get; set; }

        internal double GetMeasurementValue()
        {
            double measurementValue = Value;

            switch (Divisor)
            {
                case Divisor.D1:
                    break;
                case Divisor.D10:
                    measurementValue /= 10;
                    break;
                case Divisor.D100:
                    measurementValue /= 100;
                    break;
                case Divisor.D1000:
                    measurementValue /= 1000;
                    break;
                case Divisor.D10000:
                    measurementValue /= 10000;
                    break;
                case Divisor.ERR_POINT:
                    measurementValue = double.NaN;
                    return measurementValue;
                case Divisor.UL:
                    measurementValue = double.NegativeZero;
                    return measurementValue;
                case Divisor.OL:
                    measurementValue = double.PositiveInfinity;
                    return measurementValue;
            }
            switch (Prefixes)
            {
                case Prefixes.Pico:
                    measurementValue *= 1e-12;
                    break;
                case Prefixes.Nano:
                    measurementValue *= 1e-9;
                    break;
                case Prefixes.Micro:
                    measurementValue *= 1e-6;
                    break;
                case Prefixes.Milli:
                    measurementValue *= 1e-3;
                    break;
                case Prefixes.None:
                    break;
                case Prefixes.Kilo:
                    measurementValue *= 1e+3;
                    break;
                case Prefixes.Mega:
                    measurementValue *= 1e+6;
                    break;
                case Prefixes.Giga:
                    measurementValue *= 1e+9;
                    break;
            }
            return measurementValue;
        }
    }
}