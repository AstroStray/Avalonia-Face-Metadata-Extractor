using MetaExtractor.Core.Services;
using MetaExtractor.Core.ViewModels;
using Moq;
using OpenCvSharp;

namespace MetaExtractor.Core.Tests;

public class ImageProcessingServiceTests
{
    [Fact]
    public void SetStrategy_ShouldAcceptValidStrategy()
    {
        // Arrange
        var service = new ImageProcessingService();
        var mockStrategy = new Mock<IImageSourceStrategy>();

        // Act & Assert (Should not throw)
        service.SetStrategy(mockStrategy.Object);
    }

    [Fact]
    public async Task StartProcessingAsync_ShouldThrowWhenNoStrategy()
    {
        // Arrange
        var service = new ImageProcessingService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.StartProcessingAsync());
    }

    [Fact]
    public async Task StopProcessingAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var service = new ImageProcessingService();

        // Act & Assert (Should not throw)
        await service.StopProcessingAsync();
    }
}

public class ShellViewModelTests
{
    private readonly Mock<AnalyzeViewModel> _mockAnalyzeViewModel;
    private readonly Mock<DataClusterViewModel> _mockDataClusterViewModel;
    private readonly Mock<SettingsViewModel> _mockSettingsViewModel;
    private readonly ShellViewModel _shellViewModel;

    public ShellViewModelTests()
    {
        // Create mocks for dependencies
        var mockImageProcessingService = new Mock<IImageProcessingService>();
        var mockImageSourceStrategy = new Mock<IImageSourceStrategy>();
        
        _mockAnalyzeViewModel = new Mock<AnalyzeViewModel>(
            mockImageProcessingService.Object, 
            mockImageSourceStrategy.Object);
        _mockDataClusterViewModel = new Mock<DataClusterViewModel>();
        _mockSettingsViewModel = new Mock<SettingsViewModel>();

        _shellViewModel = new ShellViewModel(
            _mockAnalyzeViewModel.Object,
            _mockDataClusterViewModel.Object,
            _mockSettingsViewModel.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithAnalyzePageAsDefault()
    {
        // Assert
        Assert.Equal(_mockAnalyzeViewModel.Object, _shellViewModel.CurrentPage);
        Assert.Equal("Analyze", _shellViewModel.CurrentPageTitle);
        Assert.Equal("Face Detection & Analysis", _shellViewModel.CurrentPageSubtitle);
        Assert.Equal("#007acc", _shellViewModel.AnalyzeButtonBackground);
        Assert.Equal("#333333", _shellViewModel.DataClusterButtonBackground);
        Assert.Equal("#333333", _shellViewModel.SettingsButtonBackground);
    }

    [Fact]
    public void NavigateToAnalyze_ShouldUpdateCurrentPageAndState()
    {
        // Act
        _shellViewModel.NavigateToAnalyzeCommand.Execute(null);

        // Assert
        Assert.Equal(_mockAnalyzeViewModel.Object, _shellViewModel.CurrentPage);
        Assert.Equal("Analyze", _shellViewModel.CurrentPageTitle);
        Assert.Equal("Face Detection & Analysis", _shellViewModel.CurrentPageSubtitle);
        Assert.Equal("#007acc", _shellViewModel.AnalyzeButtonBackground);
        Assert.Equal("#333333", _shellViewModel.DataClusterButtonBackground);
        Assert.Equal("#333333", _shellViewModel.SettingsButtonBackground);
    }

    [Fact]
    public void NavigateToDataCluster_ShouldUpdateCurrentPageAndState()
    {
        // Act
        _shellViewModel.NavigateToDataClusterCommand.Execute(null);

        // Assert
        Assert.Equal(_mockDataClusterViewModel.Object, _shellViewModel.CurrentPage);
        Assert.Equal("Data Cluster", _shellViewModel.CurrentPageTitle);
        Assert.Equal("Metadata Analysis & Clustering", _shellViewModel.CurrentPageSubtitle);
        Assert.Equal("#333333", _shellViewModel.AnalyzeButtonBackground);
        Assert.Equal("#007acc", _shellViewModel.DataClusterButtonBackground);
        Assert.Equal("#333333", _shellViewModel.SettingsButtonBackground);
    }

    [Fact]
    public void NavigateToSettings_ShouldUpdateCurrentPageAndState()
    {
        // Act
        _shellViewModel.NavigateToSettingsCommand.Execute(null);

        // Assert
        Assert.Equal(_mockSettingsViewModel.Object, _shellViewModel.CurrentPage);
        Assert.Equal("Settings", _shellViewModel.CurrentPageTitle);
        Assert.Equal("Application Configuration", _shellViewModel.CurrentPageSubtitle);
        Assert.Equal("#333333", _shellViewModel.AnalyzeButtonBackground);
        Assert.Equal("#333333", _shellViewModel.DataClusterButtonBackground);
        Assert.Equal("#007acc", _shellViewModel.SettingsButtonBackground);
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
        _viewModel = new AnalyzeViewModel(_mockImageProcessingService.Object, _mockImageSourceStrategy.Object);
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
    public void Constructor_ShouldSetupImageProcessingService()
    {
        // Assert - Service should be set up with strategy
        _mockImageProcessingService.Verify(s => s.SetStrategy(_mockImageSourceStrategy.Object), Times.Once);
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