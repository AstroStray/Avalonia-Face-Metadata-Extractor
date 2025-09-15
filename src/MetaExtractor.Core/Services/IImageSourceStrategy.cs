using System.Threading.Tasks;
using OpenCvSharp;

namespace MetaExtractor.Core.Services;

/// <summary>
/// Defines a strategy for acquiring image frames from a source.
/// </summary>
public interface IImageSourceStrategy
{
    /// <summary>
    /// Asynchronously gets the next image frame from the source.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the next Mat frame, or null if the stream has ended.
    /// </returns>
    Task<Mat?> GetNextFrameAsync();
}
