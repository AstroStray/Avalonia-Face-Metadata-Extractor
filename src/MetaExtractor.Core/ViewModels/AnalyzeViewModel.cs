using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetaExtractor.Core.Services;
using MetaExtractor.Domain.Entities;
using OpenCvSharp;
using System.Text;
using System.Threading.Tasks;

namespace MetaExtractor.Core.ViewModels
{
    public partial class AnalyzeViewModel : ObservableObject
    {
        private readonly IImageProcessingService _imageProcessingService;

        [ObservableProperty]
        private Mat? _videoFrame;

        [ObservableProperty]
        private string _landmarks = string.Empty;

        [ObservableProperty]
        private string _expressions = string.Empty;

        [ObservableProperty]
        private string _demographics = string.Empty;

        public AnalyzeViewModel(IImageProcessingService imageProcessingService)
        {
            _imageProcessingService = imageProcessingService;

            // Subscribe to processing events
            _imageProcessingService.OnNewFrame += (frame) => VideoFrame = frame;
            _imageProcessingService.OnNewMetadata += OnMetadataReceived;
            _imageProcessingService.OnFaceDetected += OnFaceDetected;
        }

        public void SetImageSourceStrategy(IImageSourceStrategy strategy)
        {
            _imageProcessingService.SetStrategy(strategy);
        }

        private void OnMetadataReceived(Metadata metadata)
        {
            // Handle general metadata
            if (metadata.Key == "FaceDetectionError")
            {
                Landmarks = $"Error: {metadata.Value}";
            }
        }

        private void OnFaceDetected(Face face)
        {
            // Handle face detection results
            var sb = new StringBuilder();
            sb.AppendLine($"Face detected at ({face.X}, {face.Y})");
            sb.AppendLine($"Size: {face.Width}x{face.Height}");
            sb.AppendLine($"Confidence: {face.Confidence:P2}");
            sb.AppendLine($"Method: {face.ProcessingMethod}");
            
            Landmarks = sb.ToString();
            
            // TODO: Implement actual expression and demographic analysis
            Expressions = "Detection Active";
            Demographics = "Analysis Pending";
        }

        [RelayCommand]
        private async Task StartAnalysisAsync()
        {
            await _imageProcessingService.StartProcessingAsync();
        }

        [RelayCommand]
        private async Task StopAnalysisAsync()
        {
            await _imageProcessingService.StopProcessingAsync();
        }

        [RelayCommand]
        private void SaveResults()
        {
            // Logic to save the analysis results
            // TODO: Implement actual save functionality
        }
    }
}
