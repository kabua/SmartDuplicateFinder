using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Media;
using SmartDuplicateFinder.Utils;

namespace SmartDuplicateFinder.Converters;

[ValueConversion(typeof(Icons), typeof(ImageSource))]
public class GetIconConverter : IValueConverter
{
	public int DefaultImageSize { get; set; } = 16;

	/// <summary>
	/// Get an image using it's <see cref="Icons"/> name.
	/// </summary>
	/// <remarks>
	/// Parameter format: <see cref="Icons"/>.Id|pixelSize
	/// For example: Icons.FloppyDrive|16, would mean show the 16x16 Floppy drive image.
	/// </remarks>
	/// <param name="value">Must be of type <see cref="Icons"/></param>
	/// <param name="targetType">Must be of type <see cref="ImageSource"/></param>
	/// <param name="parameter">int pixelSize otherwise <see cref="DefaultImageSize"/></param>
	/// <param name="culture">Not used</param>
	/// <returns><see cref="ImageSource"/></returns>
	public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType != typeof(ImageSource))
			return Binding.DoNothing;

		Icons icons = (Icons) value;
		int pixelSize = parameter == null ? DefaultImageSize : (int) parameter;

		ImageSource? ret = IconManager.GetIcon(icons, pixelSize);
		return ret!;
	}

	/// <summary>
	/// Not supported
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

	[DllImport("shell32.dll")]
	private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}