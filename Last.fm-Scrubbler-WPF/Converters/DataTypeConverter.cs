using System;
using System.Globalization;
using System.Windows.Data;

namespace Last.fm_Scrubbler_WPF.Converters
{
  public class DataTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      return value.GetType();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}