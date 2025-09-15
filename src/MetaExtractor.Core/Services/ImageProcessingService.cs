using System.Threading.Tasks;
using OpenCvSharp;

namespace MetaExtractor.Core.Services;

public class ImageProcessingService : IImageProcessingService
{
    private IImageSourceStrategy? _strategy;

    public void SetStrategy(IImageSourceStrategy strategy)
    {
        _strategy = strategy;
    }

    public async Task<Mat?> ProcessNextFrameAsync()
    {
        if (_strategy is null)
        {
            return null;
        }

        var frame = await _strategy.GetNextFrameAsync();

        if (frame is not null)
        {
            // In the future, MediaPipe processing logic will go here.
        }

        return frame;
    }
}
