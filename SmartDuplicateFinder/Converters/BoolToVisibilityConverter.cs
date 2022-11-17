using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartDuplicateFinder.Converters;

/// <summary>
/// Convert a <see cref="bool"/> value to <see cref="Visibility"/> value, depending on the values of <see cref="TrueValue"/> and <see cref="FalseValue"/>
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Get or set what the <see cref="Visibility"/> is when the <see cref="bool"/> value is true. Default: <see cref="Visibility.Visible"/>
    /// </summary>
    public Visibility TrueValue { get; set; }

    /// <summary>
    /// Get or set what the <see cref="Visibility"/> is when the <see cref="bool"/> value is false. Default: <see cref="Visibility.Collapsed"/>
    /// </summary>
    public Visibility FalseValue { get; set; }

    public BoolToVisibilityConverter()
    {
        // set defaults
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }

    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to test</param>
    /// <param name="targetType">A <see cref="Visibility"/></param>
    /// <param name="parameter">Not used</param>
    /// <param name="culture">Not used</param>
    /// <returns>The <see cref="Visibility"/> value as defined by <see cref="TrueValue"/> when the <paramref name="value"/> is true; otherwise the <see cref="FalseValue"/>.</returns>
    public object? Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Visibility))
            return null;

        if (value is not bool b)
            return null;

        return b ? TrueValue : FalseValue;
    }

    /// <summary>
    /// Can convert back.
    /// </summary>
    /// <param name="value">A <see cref="Visibility"/> value</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Not Used</param>
    /// <param name="culture"></param>
    /// <returns>If the <paramref name="value"/> matches the <see cref="TrueValue"/> then return true, if it matches the <see cref="FalseValue"/> then return false; otherwise return null.</returns>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Equals(value, TrueValue))
            return true;
        if (Equals(value, FalseValue))
            return false;

        return null;
    }
}