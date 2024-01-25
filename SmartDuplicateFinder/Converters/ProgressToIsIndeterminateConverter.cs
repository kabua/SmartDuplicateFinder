using SmartDuplicateFinder.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SmartDuplicateFinder.Converters;

/// <summary>
/// If <see cref="IProgress.Current"/> or <see cref="IProgress.Total"/> are either NAN or Infinity return true otherwise false. Useful for setting <see cref="System.Windows.Controls.ProgressBar.IsIndeterminate"/>
/// </summary>
public sealed class ProgressToIsIndeterminateConverter : IMultiValueConverter
{
	/// <summary>
	/// Convert
	/// </summary>
	/// <remarks>
	/// <code>
	/// <ProgressBar.IsIndeterminate>
	///     <MultiBinding Converter="{StaticResource _isIndeterminateConverter}">
	///         <MultiBinding.Bindings>
	///             <Binding Path="StepProgress.Current" />
	///             <Binding Path="StepProgress.Total" />
	///         </MultiBinding.Bindings>
	///     </MultiBinding>
	/// </ProgressBar.IsIndeterminate>
	/// </code>
	/// </remarks>
	/// <param name="values"><see cref="IProgress.Current"/> and <see cref="IProgress.Total"/></param>
	/// <param name="targetType">of type <see cref="bool"/></param>
	/// <param name="parameter">Not used</param>
	/// <param name="culture">Not used</param>
	/// <returns>True if <see cref="IProgress.Current"/> or <see cref="IProgress.Total"/> is either NAN or Infinity; otherwise false</returns>
	public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if (targetType != typeof(bool))
			return null;

		var results = values.Cast<double>().Any(v => double.IsNaN(v) || double.IsInfinity(v));
		return results;
	}

	/// <summary>
	/// Not supported.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}