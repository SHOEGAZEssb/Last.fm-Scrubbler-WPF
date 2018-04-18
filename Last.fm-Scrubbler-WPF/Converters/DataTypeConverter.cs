using System;
using System.Globalization;
using System.Windows.Data;

namespace Scrubbler.Converters
{
  /// <summary>
  /// Converter that converts an object to its type.
  /// </summary>
  public class DataTypeConverter : IValueConverter
  {
    /// <summary>
    /// Converts the given <paramref name="value"/> to its type.
    /// </summary>
    /// <param name="value">Object to convert.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="culture">Ignored.</param>
    /// <returns>Type of the given <paramref name="value"/></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      return value.GetType();
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="value">Ignored.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="culture">Ignored.</param>
    /// <returns>Nothing.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}