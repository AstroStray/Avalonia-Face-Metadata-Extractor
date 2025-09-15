namespace MetaExtractor.Core.Services;

public interface IImageSourceStrategyFactory
{
    IImageSourceStrategy CreateFileStrategy(string filePath);
    IImageSourceStrategy CreateCameraStrategy(int cameraIndex = 0);
}
