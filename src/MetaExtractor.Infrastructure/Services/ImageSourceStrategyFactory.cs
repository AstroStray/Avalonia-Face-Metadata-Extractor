using MetaExtractor.Core.Services;

namespace MetaExtractor.Infrastructure.Services;

public class ImageSourceStrategyFactory : IImageSourceStrategyFactory
{
    public IImageSourceStrategy CreateFileStrategy(string filePath)
    {
        return new FileImageSourceStrategy(filePath);
    }

    public IImageSourceStrategy CreateCameraStrategy(int cameraIndex = 0)
    {
        return new CameraImageSourceStrategy(cameraIndex);
    }
}
