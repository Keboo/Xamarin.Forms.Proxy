using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Proxy;

namespace Example
{
    public class ColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 3 && values.Select(x => x?.ToString()).All(x => x != null))
            {
                return Color.FromHex(string.Join("", values));
            }
            return Color.Transparent;
        }
    }
}