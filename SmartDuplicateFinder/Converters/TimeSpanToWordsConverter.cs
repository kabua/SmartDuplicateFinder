using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartDuplicateFinder.Converters;

/// <summary>
/// Convert a <see cref="TimeSpan"/> in to sentence. I.e. 5:01:43 would return "5 hours 1 minute 43 seconds"
/// </summary>
[ValueConversion(typeof (TimeSpan), typeof (string))]
public sealed class TimeSpanToWordsConverter : IValueConverter
{
	/// <summary>
	/// Convert
	/// </summary>
	/// <param name="value">The <see cref="TimeSpan"/> to convert to words</param>
	/// <param name="targetType">A string</param>
	/// <param name="parameter">Not used</param>
	/// <param name="culture">Not used</param>
	/// <returns>The <see cref="TimeSpan"/> as string of words.</returns>
	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (targetType != typeof(string))
			return null;

		if (value is not TimeSpan timeSpan)
			return null;

		static string FormatTimePart(int item, string itemName, string code) => item > 0 ? "{0:%" + code + "} " + itemName + (item > 1 ? "s " : " ") : "";

		var format = "";
		format += FormatTimePart(timeSpan.Days, "day", "d");
		format += FormatTimePart(timeSpan.Hours, "hour", "h");
		format += FormatTimePart(timeSpan.Minutes, "minute", "m");
		format += FormatTimePart(timeSpan.Seconds, "second", "s");

		return string.Format(format, timeSpan);
	}

	/// <summary>
	/// Not supported
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}