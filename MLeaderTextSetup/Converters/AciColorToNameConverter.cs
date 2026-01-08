using System;
using System.Globalization;
using System.Windows.Data;

namespace MLeaderTextSetup.Converters
{
    public class AciColorToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short aciIndex)
            {
                return AciIndexToName(aciIndex);
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string AciIndexToName(short aciIndex)
        {
            switch (aciIndex)
            {
                case 1: return "Red";
                case 2: return "Yellow";
                case 3: return "Green";
                case 4: return "Cyan";
                case 5: return "Blue";
                case 6: return "Magenta";
                case 7: return "White";
                case 8: return "Gray";
                case 9: return "Light Gray";
                case 256: return "ByLayer";
                default: return $"Color {aciIndex}";
            }
        }
    }
}
