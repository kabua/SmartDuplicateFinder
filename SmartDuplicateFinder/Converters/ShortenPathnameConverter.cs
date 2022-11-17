using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SmartDuplicateFinder.Extensions;

namespace SmartDuplicateFinder.Converters;

/// <summary>
/// Shorten a path to fit within the width of the <see cref="TextBlock"/>. The same instance can be used with multiple <see cref="TextBlock"/>.
/// </summary>
public sealed class ShortenPathnameConverter : IMultiValueConverter
{
    /// <summary>
    /// Convert
    /// </summary>
    /// <remarks>
    /// <code>
    /// <TextBlock Grid.Row="3">
    ///     <TextBlock.Text>
    ///         <MultiBinding Converter="{StaticResource _shortenPathnameConverter}">
    ///             <MultiBinding.Bindings>
    ///                 <Binding RelativeSource="{x:Static RelativeSource.Self}"/>
    ///                 <Binding Path="StepProgress.Description" />
    ///             </MultiBinding.Bindings>
    ///             </MultiBinding>
    ///     </TextBlock.Text>
    /// </TextBlock>
    /// </code>
    /// </remarks>
    /// <param name="value">two values; 1) the <see cref="TextBlock"/>, 2) the pathname to shorten.</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Not used</param>
    /// <param name="culture"></param>
    /// <returns>If required, a shorten Pathname otherwise the original value</returns>
    public object? Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
            return null;

        if (value[0] is not TextBlock textBlock)
            return null;

        if (value[1] is not string source)
            return null;

        if (textBlock.ActualWidth == 0)
        {
            // Not initialized yet.
            //
            return source;
        }  

        if (!_cache.TryGetValue(textBlock, out var aveCharWidth))
        {
            var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);

            var formattedText = new FormattedText(AverageSizeChars, culture, FlowDirection.LeftToRight, typeface, textBlock.FontSize,
                Brushes.Black, VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);

            aveCharWidth = formattedText.Width / 2;

            _cache[textBlock] = aveCharWidth;
        }

        var maxChars = (int) (textBlock.ActualWidth / aveCharWidth);
        var shortenPathName = source.ShortenPathname(maxChars);

        return shortenPathName;
    }

    /// <summary>
    /// Not supported
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

    // The true calculated average is slightly larger than "ET".Width() / 2.0, which is good so that the right side of the string will not be truncated due to rounding errors.
    //
    private const string AverageSizeChars = "ET";   

    private readonly Dictionary<TextBlock, double> _cache = new ();
}