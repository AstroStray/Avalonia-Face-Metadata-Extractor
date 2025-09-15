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

        public AnalyzeViewModel(IImageProcessingService imageProcessingService, IImageSourceStrategy imageSourceStrategy)
        {
            _imageProcessingService = imageProcessingService;
            _imageProcessingService.SetStrategy(imageSourceStrategy);

            _imageProcessingService.OnNewFrame += (frame) => VideoFrame = frame;
            _imageProcessingService.OnNewMetadata += (metadata) =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("Face detected.");
                Landmarks = sb.ToString();
                Expressions = "Happy";
                Demographics = "Male, 25-30";
            };
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
