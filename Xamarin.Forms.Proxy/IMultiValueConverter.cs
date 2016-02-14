using System;
using System.Globalization;

namespace Xamarin.Forms.Proxy
{
    public interface IMultiValueConverter
    {
        object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
    }
}