using SmartDuplicateFinder.Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartDuplicateFinder.Converters;

/// <summary>
/// If <see cref="IProgress.Current"/> or <see cref="IProgress.Total"/> are either NAN or Infinity return true otherwise false. Useful for setting <see cref="System.Windows.Controls.ProgressBar.IsIndeterminate"/>
/// </summary>
[ValueConversion(typeof (IProgress), typeof (bool))]
public sealed class ProgressToIsIndeterminateConverter : IValueConverter
{
	/// <summary>
	/// Convert
	/// </summary>
	/// <param name="value">The <see cref="IProgress"/> to test</param>
	/// <param name="targetType">of type <see cref="bool"/></param>
	/// <param name="parameter">Not used</param>
	/// <param name="culture">Not used</param>
	/// <returns>True if <see cref="IProgress.Current"/> or <see cref="IProgress.Total"/> is either NAN or Infinity; otherwise false</returns>
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (targetType != typeof(bool))
			return null;

		if (value is not IProgress progress)
			return null;

		return double.IsNaN(progress.Current) || double.IsNaN(progress.Total) || double.IsInfinity(progress.Current) || double.IsInfinity(progress.Total);
	}

	/// <summary>
	/// Not supported.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}