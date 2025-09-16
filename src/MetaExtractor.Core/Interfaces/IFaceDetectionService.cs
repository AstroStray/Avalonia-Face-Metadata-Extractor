using MetaExtractor.Core.Models;
using OpenCvSharp;
using System.Threading.Tasks;

namespace MetaExtractor.Core.Interfaces;

public interface IFaceDetectionService
{
    Task<FaceDetectionResult> DetectFacesAsync(Mat image, string? imagePath = null);
    Task<FaceDetectionResult> DetectFacesAsync(string imagePath);
    Task<bool> IsInitializedAsync();
    void Dispose();
}