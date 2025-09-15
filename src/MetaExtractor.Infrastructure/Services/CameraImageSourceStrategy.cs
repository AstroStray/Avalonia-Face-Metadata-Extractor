using System;
using System.Threading.Tasks;
using MetaExtractor.Core.Services;
using OpenCvSharp;

namespace MetaExtractor.Infrastructure.Services;

public class CameraImageSourceStrategy : IImageSourceStrategy, IDisposable
{
    private readonly VideoCapture _capture;

    public CameraImageSourceStrategy(int cameraIndex = 0)
    {
        _capture = new VideoCapture(cameraIndex);
        if (!_capture.IsOpened())
        {
            throw new InvalidOperationException($"Could not open camera with index {cameraIndex}.");
        }
    }

    public Task<Mat?> GetNextFrameAsync()
    {
        var frame = new Mat();
        _capture.Read(frame);

        if (frame.Empty())
        {
            return Task.FromResult<Mat?>(null);
        }

        return Task.FromResult<Mat?>(frame);
    }

    public void Dispose()
    {
        _capture.Dispose();
    }
}
