using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MetaExtractor.Core.Services;
using OpenCvSharp;

namespace MetaExtractor.Infrastructure.Services;

public class FileImageSourceStrategy : IImageSourceStrategy, IDisposable
{
    private readonly string _filePath;
    private readonly bool _isVideoFile;
    private VideoCapture? _videoCapture;
    private bool _isImageRead;
    private bool _disposed;

    public FileImageSourceStrategy(string filePath)
    {
        _filePath = filePath;
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        _isVideoFile = IsVideoFile(filePath);
        
        if (_isVideoFile)
        {
            _videoCapture = new VideoCapture(filePath);
            if (!_videoCapture.IsOpened())
            {
                throw new InvalidOperationException($"Could not open video file: {filePath}");
            }
        }
    }

    public Task<Mat?> GetNextFrameAsync()
    {
        if (_disposed)
            return Task.FromResult<Mat?>(null);

        if (_isVideoFile)
        {
            return GetNextVideoFrameAsync();
        }
        else
        {
            return GetImageFrameAsync();
        }
    }

    private Task<Mat?> GetNextVideoFrameAsync()
    {
        if (_videoCapture == null || !_videoCapture.IsOpened())
            return Task.FromResult<Mat?>(null);

        var frame = new Mat();
        try
        {
            _videoCapture.Read(frame);

            if (frame.Empty())
            {
                frame.Dispose();
                return Task.FromResult<Mat?>(null);
            }

            return Task.FromResult<Mat?>(frame);
        }
        catch
        {
            frame.Dispose();
            return Task.FromResult<Mat?>(null);
        }
    }

    private Task<Mat?> GetImageFrameAsync()
    {
        // For a single image file, we only read it once
        if (_isImageRead)
        {
            return Task.FromResult<Mat?>(null);
        }

        _isImageRead = true;

        Mat? mat = null;
        try
        {
            mat = new Mat(_filePath, ImreadModes.Color);

            if (mat.Empty())
            {
                mat.Dispose();
                return Task.FromResult<Mat?>(null);
            }

            return Task.FromResult<Mat?>(mat);
        }
        catch (Exception)
        {
            mat?.Dispose();
            return Task.FromResult<Mat?>(null);
        }
    }

    private static bool IsVideoFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".mp4" => true,
            ".avi" => true,
            ".mov" => true,
            ".mkv" => true,
            ".wmv" => true,
            ".flv" => true,
            ".webm" => true,
            _ => false
        };
    }

    /// <summary>
    /// Gets supported file extensions.
    /// </summary>
    public static string[] GetSupportedImageExtensions() => 
        [".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif", ".webp"];

    /// <summary>
    /// Gets supported video extensions.
    /// </summary>
    public static string[] GetSupportedVideoExtensions() => 
        [".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv", ".webm"];

    /// <summary>
    /// Checks if a file is supported by this strategy.
    /// </summary>
    public static bool IsFileSupported(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return GetSupportedImageExtensions().Contains(extension) || 
               GetSupportedVideoExtensions().Contains(extension);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _videoCapture?.Dispose();
            _disposed = true;
        }
    }
}
