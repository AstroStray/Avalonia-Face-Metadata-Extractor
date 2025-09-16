using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MetaExtractor.Core.Interfaces;
using MetaExtractor.Domain.Entities;
using OpenCvSharp;

namespace MetaExtractor.Core.Services
{
    public class ImageProcessingService : IImageProcessingService
    {
        private IImageSourceStrategy? _strategy;
        private readonly IFaceDetectionService _faceDetectionService;
        private CancellationTokenSource? _cancellationTokenSource;

        public event Action<Mat>? OnNewFrame;
        public event Action<Metadata>? OnNewMetadata;
        public event Action<Face>? OnFaceDetected;

        public ImageProcessingService(IFaceDetectionService faceDetectionService)
        {
            _faceDetectionService = faceDetectionService;
        }

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

                        // Perform face detection
                        await ProcessFaceDetectionAsync(frame);
                    }
                    await Task.Delay(33, token); // ~30 FPS
                }
            }, token);
        }

        private async Task ProcessFaceDetectionAsync(Mat frame)
        {
            try
            {
                var faceDetectionResult = await _faceDetectionService.DetectFacesAsync(frame);
                
                if (faceDetectionResult.Success)
                {
                    foreach (var detectedFace in faceDetectionResult.Faces)
                    {
                        // Create Face entity
                        var face = new Face
                        {
                            Timestamp = DateTime.UtcNow,
                            X = detectedFace.BoundingBox.X,
                            Y = detectedFace.BoundingBox.Y,
                            Width = detectedFace.BoundingBox.Width,
                            Height = detectedFace.BoundingBox.Height,
                            Confidence = detectedFace.Confidence,
                            ImagePath = detectedFace.ImagePath,
                            ProcessingMethod = faceDetectionResult.ProcessingMethod,
                            LandmarkData = JsonSerializer.Serialize(detectedFace.Landmarks)
                        };

                        OnFaceDetected?.Invoke(face);

                        // Create metadata for the detected face
                        var metadata = new Metadata
                        {
                            Timestamp = DateTime.UtcNow,
                            Key = "FaceDetection",
                            Value = $"Face detected at ({face.X},{face.Y}) with size {face.Width}x{face.Height}",
                            FaceId = face.Id
                        };

                        OnNewMetadata?.Invoke(metadata);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't stop processing
                var errorMetadata = new Metadata
                {
                    Timestamp = DateTime.UtcNow,
                    Key = "FaceDetectionError",
                    Value = ex.Message
                };
                OnNewMetadata?.Invoke(errorMetadata);
            }
        }

        public Task StopProcessingAsync()
        {
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }
    }
}