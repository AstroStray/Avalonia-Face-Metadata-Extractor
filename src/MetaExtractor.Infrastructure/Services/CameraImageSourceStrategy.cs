using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaExtractor.Core.Services;
using OpenCvSharp;

namespace MetaExtractor.Infrastructure.Services;

public class CameraImageSourceStrategy : IImageSourceStrategy, IDisposable
{
    private readonly VideoCapture _capture;
    private readonly int _cameraIndex;
    private bool _isInitialized;

    public CameraImageSourceStrategy(int cameraIndex = 0)
    {
        _cameraIndex = cameraIndex;
        _capture = new VideoCapture(cameraIndex);
        
        if (!_capture.IsOpened())
        {
            throw new InvalidOperationException($"Could not open camera with index {cameraIndex}.");
        }
        
        InitializeCamera();
    }

    /// <summary>
    /// Gets available camera devices.
    /// </summary>
    /// <returns>List of available camera indices.</returns>
    public static List<int> GetAvailableCameras()
    {
        var availableCameras = new List<int>();
        
        // Test up to 10 camera indices
        for (int i = 0; i < 10; i++)
        {
            using var testCapture = new VideoCapture(i);
            if (testCapture.IsOpened())
            {
                availableCameras.Add(i);
            }
        }
        
        return availableCameras;
    }

    /// <summary>
    /// Gets camera information.
    /// </summary>
    public CameraInfo GetCameraInfo()
    {
        if (!_capture.IsOpened())
            return new CameraInfo(_cameraIndex, "Unavailable", 0, 0, 0);

        var width = (int)_capture.Get(VideoCaptureProperties.FrameWidth);
        var height = (int)_capture.Get(VideoCaptureProperties.FrameHeight);
        var fps = _capture.Get(VideoCaptureProperties.Fps);

        return new CameraInfo(_cameraIndex, $"Camera {_cameraIndex}", width, height, fps);
    }

    private void InitializeCamera()
    {
        if (_isInitialized || !_capture.IsOpened()) return;

        // Set optimal camera settings
        _capture.Set(VideoCaptureProperties.FrameWidth, 640);
        _capture.Set(VideoCaptureProperties.FrameHeight, 480);
        _capture.Set(VideoCaptureProperties.Fps, 30);
        _capture.Set(VideoCaptureProperties.BufferSize, 1); // Reduce buffer to get latest frame
        
        _isInitialized = true;
    }

    public Task<Mat?> GetNextFrameAsync()
    {
        var frame = new Mat();
        try
        {
            _capture.Read(frame);

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

    public void Dispose()
    {
        _capture.Dispose();
    }
}

/// <summary>
/// Contains information about a camera device.
/// </summary>
/// <param name="Index">Camera device index.</param>
/// <param name="Name">Camera device name.</param>
/// <param name="Width">Frame width.</param>
/// <param name="Height">Frame height.</param>
/// <param name="Fps">Frames per second.</param>
public record CameraInfo(int Index, string Name, int Width, int Height, double Fps);
