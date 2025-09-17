using MetaExtractor.Core.Interfaces;
using MetaExtractor.Core.Services;
using MetaExtractor.Core.ViewModels;
using MetaExtractor.Domain.Entities;
using Moq;
using OpenCvSharp;

namespace MetaExtractor.Core.Tests;

public class ImageProcessingServiceTests
{
    private readonly Mock<IFaceDetectionService> _mockFaceDetectionService;

    public ImageProcessingServiceTests()
    {
        _mockFaceDetectionService = new Mock<IFaceDetectionService>();
    }

    [Fact]
    public void SetStrategy_ShouldAcceptValidStrategy()
    {
        // Arrange
        var service = new ImageProcessingService(_mockFaceDetectionService.Object);
        var mockStrategy = new Mock<IImageSourceStrategy>();

        // Act & Assert (Should not throw)
        service.SetStrategy(mockStrategy.Object);
    }

    [Fact]
    public async Task StartProcessingAsync_ShouldThrowWhenNoStrategy()
    {
        // Arrange
        var service = new ImageProcessingService(_mockFaceDetectionService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.StartProcessingAsync());
    }

    [Fact]
    public async Task StopProcessingAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var service = new ImageProcessingService(_mockFaceDetectionService.Object);

        // Act & Assert (Should not throw)
        await service.StopProcessingAsync();
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Act & Assert (Should not throw)
        var service = new ImageProcessingService(_mockFaceDetectionService.Object);
        Assert.NotNull(service);
    }
}

public class ShellViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly ShellViewModel _shellViewModel;

    public ShellViewModelTests()
    {
        _mockNavigationService = new Mock<INavigationService>();
        _shellViewModel = new ShellViewModel(_mockNavigationService.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithNavigationService()
    {
        // Assert
        Assert.NotNull(_shellViewModel);
        // Verify navigation service was called on initialization
        _mockNavigationService.Verify(x => x.NavigateAsync("Analyze", null), Times.Once);
    }

    [Fact]
    public async Task NavigateToAnalyze_ShouldCallNavigationService()
    {
        // Act
        await _shellViewModel.NavigateToAnalyzeCommand.ExecuteAsync(null);

        // Assert
        _mockNavigationService.Verify(x => x.NavigateAsync("Analyze", null), Times.AtLeastOnce);
    }

    [Fact]
    public async Task NavigateToDataCluster_ShouldCallNavigationService()
    {
        // Act
        await _shellViewModel.NavigateToDataClusterCommand.ExecuteAsync(null);

        // Assert
        _mockNavigationService.Verify(x => x.NavigateAsync("DataCluster", null), Times.Once);
    }

    [Fact]
    public async Task NavigateToSettings_ShouldCallNavigationService()
    {
        // Act
        await _shellViewModel.NavigateToSettingsCommand.ExecuteAsync(null);

        // Assert
        _mockNavigationService.Verify(x => x.NavigateAsync("Settings", null), Times.Once);
    }

    [Fact]
    public void OnNavigationRequested_ShouldUpdateUIForAnalyzePage()
    {
        // Arrange
        var mockAnalyzeViewModel = new Mock<AnalyzeViewModel>(Mock.Of<IImageProcessingService>());
        var eventArgs = new NavigationEventArgs("Analyze", mockAnalyzeViewModel.Object);

        // Act - Simulate navigation event
        _mockNavigationService.Raise(x => x.NavigationRequested += null, _mockNavigationService.Object, eventArgs);

        // Assert
        Assert.Equal("Analyze", _shellViewModel.CurrentPageTitle);
        Assert.Equal("Face Detection & Analysis", _shellViewModel.CurrentPageSubtitle);
        Assert.Equal("#007acc", _shellViewModel.AnalyzeButtonBackground);
        Assert.Equal("#333333", _shellViewModel.DataClusterButtonBackground);
        Assert.Equal("#333333", _shellViewModel.SettingsButtonBackground);
    }
}

public class AnalyzeViewModelTests
{
    private readonly Mock<IImageProcessingService> _mockImageProcessingService;
    private readonly Mock<IImageSourceStrategy> _mockImageSourceStrategy;
    private readonly AnalyzeViewModel _viewModel;

    public AnalyzeViewModelTests()
    {
        _mockImageProcessingService = new Mock<IImageProcessingService>();
        _mockImageSourceStrategy = new Mock<IImageSourceStrategy>();
        _viewModel = new AnalyzeViewModel(_mockImageProcessingService.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Assert
        Assert.Null(_viewModel.VideoFrame);
        Assert.Equal(string.Empty, _viewModel.Landmarks);
        Assert.Equal(string.Empty, _viewModel.Expressions);
        Assert.Equal(string.Empty, _viewModel.Demographics);
    }

    [Fact]
    public void Constructor_ShouldSubscribeToProcessingEvents()
    {
        // Assert - Should subscribe to all processing events
        _mockImageProcessingService.VerifyAdd(s => s.OnNewFrame += It.IsAny<Action<Mat>>(), Times.Once);
        _mockImageProcessingService.VerifyAdd(s => s.OnNewMetadata += It.IsAny<Action<Metadata>>(), Times.Once);
        _mockImageProcessingService.VerifyAdd(s => s.OnFaceDetected += It.IsAny<Action<Face>>(), Times.Once);
    }

    [Fact]
    public async Task StartAnalysisCommand_ShouldCallImageProcessingService()
    {
        // Act
        await _viewModel.StartAnalysisCommand.ExecuteAsync(null);

        // Assert
        _mockImageProcessingService.Verify(s => s.StartProcessingAsync(), Times.Once);
    }

    [Fact]
    public async Task StopAnalysisCommand_ShouldCallImageProcessingService()
    {
        // Act
        await _viewModel.StopAnalysisCommand.ExecuteAsync(null);

        // Assert
        _mockImageProcessingService.Verify(s => s.StopProcessingAsync(), Times.Once);
    }

    [Fact]
    public void SaveResultsCommand_ShouldExecuteWithoutException()
    {
        // Act & Assert (Should not throw)
        _viewModel.SaveResultsCommand.Execute(null);
    }
}