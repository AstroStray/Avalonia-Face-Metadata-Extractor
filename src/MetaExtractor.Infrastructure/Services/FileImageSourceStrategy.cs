using System.Threading.Tasks;
using MetaExtractor.Core.Services;
using OpenCvSharp;

namespace MetaExtractor.Infrastructure.Services;

public class FileImageSourceStrategy : IImageSourceStrategy
{
    private readonly string _filePath;
    private bool _isRead;

    public FileImageSourceStrategy(string filePath)
    {
        _filePath = filePath;
        _isRead = false;
    }

    public Task<Mat?> GetNextFrameAsync()
    {
        // For a single file, we only read it once.
        if (_isRead)
        {
            return Task.FromResult<Mat?>(null);
        }

        _isRead = true;
        var mat = new Mat(_filePath, ImreadModes.Color);

        if (mat.Empty())
        {
            // Return null if the image is empty or couldn't be read
            return Task.FromResult<Mat?>(null);
        }

        return Task.FromResult<Mat?>(mat);
    }
}
