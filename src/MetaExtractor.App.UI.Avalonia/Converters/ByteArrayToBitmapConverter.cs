using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace MetaExtractor.App.UI.Avalonia.Converters;

public class ByteArrayToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte[] bytes && bytes.Length > 0)
        {
            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    return new Bitmap(stream);
                }
            }
            catch
            {
                // Return null if conversion fails
                return null;
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
