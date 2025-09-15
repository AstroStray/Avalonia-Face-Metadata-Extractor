using System.Threading.Tasks;
using OpenCvSharp;

namespace MetaExtractor.Core.Services;

public interface IImageProcessingService
{
    void SetStrategy(IImageSourceStrategy strategy);
    Task<Mat?> ProcessNextFrameAsync();
}
