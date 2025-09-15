using System;
using System.Threading;
using System.Threading.Tasks;
using MetaExtractor.Domain.Entities;
using OpenCvSharp;

namespace MetaExtractor.Core.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private IImageSourceStrategy? _strategy;
        private CancellationTokenSource? _cancellationTokenSource;

        public event Action<Mat>? OnNewFrame;
        public event Action<Metadata>? OnNewMetadata;

        public void SetStrategy(IImageSourceStrategy strategy)
        {
            _strategy = strategy;
        }

        public Task StartProcessingAsync()
        {
            if (_strategy is null)
            {
                throw new InvalidOperationException("Image source strategy not set.");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var frame = await _strategy.GetNextFrameAsync();
                    if (frame != null)
                    {
                        OnNewFrame?.Invoke(frame);

                        // Placeholder for MediaPipe processing
                        var metadata = new Metadata { /* Populate with data */ };
                        OnNewMetadata?.Invoke(metadata);
                    }
                    await Task.Delay(33, token); // ~30 FPS
                }
            }, token);
        }

        public Task StopProcessingAsync()
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }
    }
}