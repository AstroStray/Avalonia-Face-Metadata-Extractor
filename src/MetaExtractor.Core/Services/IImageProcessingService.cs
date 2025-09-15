using System;
using System.Threading.Tasks;
using MetaExtractor.Domain.Entities;
using OpenCvSharp;

namespace MetaExtractor.Core.Services
{
    public interface IImageProcessingService
    {
        event Action<Mat>? OnNewFrame;
        event Action<Metadata>? OnNewMetadata;

        void SetStrategy(IImageSourceStrategy strategy);
        Task StartProcessingAsync();
        Task StopProcessingAsync();
    }
}