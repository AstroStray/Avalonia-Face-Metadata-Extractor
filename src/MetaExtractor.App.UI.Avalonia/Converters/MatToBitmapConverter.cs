using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using OpenCvSharp;

namespace MetaExtractor.App.UI.Avalonia.Converters
{
    public class MatToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Mat mat || mat.Empty())
                return null;

            try
            {
                // Convert Mat to bytes in a format suitable for Avalonia
                var imageBytes = mat.ImEncode(".png");
                
                // Create Avalonia Bitmap from byte array
                using var stream = new MemoryStream(imageBytes);
                return new Bitmap(stream);
            }
            catch
            {
                return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converting from Bitmap to Mat is not supported.");
        }
    }
}
