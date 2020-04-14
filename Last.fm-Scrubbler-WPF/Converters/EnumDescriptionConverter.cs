using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Scrubbler.Converters
{
  /// <summary>
  /// Converter to convert the description attribute of an enum to a string.
  /// </summary>
  public class EnumDescriptionConverter : IValueConverter
  {
    /// <summary>
    /// Gets the enum description of the given <paramref name="enumObj"/>.
    /// </summary>
    /// <param name="enumObj">Enum to get the description of.</param>
    /// <returns>Description as string.</returns>
    private static string GetEnumDescription(Enum enumObj)
    {
      FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
      object[] attribArray = fieldInfo.GetCustomAttributes(false);

      if (attribArray.Length == 0)
        return enumObj.ToString();
      else
        return (attribArray[0] as DescriptionAttribute).Description;
    }

    /// <summary>
    /// Converts the given <paramref name="value"/> to its enum description.
    /// </summary>
    /// <param name="value">Enum to convert.</param>
    /// <param name="targetType">Ignored.</param>
    /// <param name="parameter">Ignored.</param>
    /// <param name="culture">Ignored.</param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || !(value is Enum myEnum))
        return "";

      string description = GetEnumDescription(myEnum);
      return description;
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