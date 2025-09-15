using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetaExtractor.Core.Models;
using MetaExtractor.Core.Services;
using OpenCvSharp;

namespace MetaExtractor.Core.ViewModels;

public partial class AnalyzeViewModel : ObservableObject
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IPythonScriptRunner _pythonRunner;
    private readonly IImageSourceStrategyFactory _strategyFactory;

    [ObservableProperty]
    private string? _resultJson;

    [ObservableProperty]
    private byte[]? _displayedImage;

    public AnalyzeViewModel(
        IImageProcessingService imageProcessingService,
        IPythonScriptRunner pythonRunner,
        IImageSourceStrategyFactory strategyFactory)
    {
        _imageProcessingService = imageProcessingService;
        _pythonRunner = pythonRunner;
        _strategyFactory = strategyFactory;
    }

    [RelayCommand]
    private async Task AnalyzeImageAsync()
    {
        try
        {
            // 1. Set up the image source strategy
            var imagePath = "scripts/test_face.jpg"; // Hard-coded for now
            var strategy = _strategyFactory.CreateFileStrategy(imagePath);
            _imageProcessingService.SetStrategy(strategy);

            // 2. Get the image frame
            var frame = await _imageProcessingService.ProcessNextFrameAsync();
            if (frame is null)
            {
                ResultJson = "Could not load image.";
                return;
            }

            // 3. Save the frame to a temporary file to pass to Python
            var tempImagePath = Path.Combine(Path.GetTempPath(), "temp_frame.jpg");
            frame.SaveImage(tempImagePath);

            // 4. Run the Python script
            var scriptPath = "scripts/face_detector.py";
            var jsonResult = await _pythonRunner.RunScriptAsync(scriptPath, $"\"{tempImagePath}\"");
            ResultJson = jsonResult; // Keep for debugging

            // 5. Deserialize the result
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var detectionResult = JsonSerializer.Deserialize<FaceDetectionResult>(jsonResult, options);

            // 6. Draw landmarks on the frame
            if (detectionResult?.Faces != null)
            {
                foreach (var face in detectionResult.Faces)
                {
                    foreach (var landmark in face)
                    {
                        var x = (int)(landmark.X * frame.Width);
                        var y = (int)(landmark.Y * frame.Height);
                        Cv2.Circle(frame, new Point(x, y), 2, new Scalar(0, 255, 0), -1);
                    }
                }
            }

            // 7. Display the modified image
            DisplayedImage = frame.ToBytes(".jpg");
            frame.Dispose();

            // 8. Clean up the temporary file
            File.Delete(tempImagePath);
        }
        catch (Exception ex)
        {
            ResultJson = $"An error occurred: {ex.Message}";
        }
    }
}
