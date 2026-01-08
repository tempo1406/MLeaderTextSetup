using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MLeaderTextSetup.Converters
{
    public class AciColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short aciIndex)
            {
                return AciIndexToBrush(aciIndex);
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Brush AciIndexToBrush(short aciIndex)
        {
            switch (aciIndex)
            {
                case 1: return Brushes.Red;
                case 2: return Brushes.Yellow;
                case 3: return Brushes.Lime;
                case 4: return Brushes.Cyan;
                case 5: return Brushes.Blue;
                case 6: return Brushes.Magenta;
                case 7: return Brushes.White;
                case 8: return Brushes.Gray;
                case 9: return Brushes.LightGray;
                case 256: return Brushes.White;
                default: return Brushes.White;
            }
        }
    }
}
