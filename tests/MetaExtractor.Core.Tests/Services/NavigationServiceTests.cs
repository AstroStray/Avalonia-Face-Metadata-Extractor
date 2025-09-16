using System;
using System.Threading.Tasks;
using MetaExtractor.Core.Services;
using MetaExtractor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MetaExtractor.Core.Tests.Services;

public class NavigationServiceTests
{
    private readonly NavigationService _navigationService;
    private readonly ServiceProvider _serviceProvider;
    private bool _navigationEventRaised;

    public NavigationServiceTests()
    {
        var services = new ServiceCollection();
        services.AddTransient<AnalyzeViewModel>(_ => 
            new AnalyzeViewModel(Mock.Of<IImageProcessingService>(), Mock.Of<IImageSourceStrategy>()));
        services.AddTransient<DataClusterViewModel>();
        services.AddTransient<SettingsViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
        _navigationService = new NavigationService(_serviceProvider);
        
        _navigationService.NavigationRequested += (_, _) => _navigationEventRaised = true;
    }

    [Fact]
    public void DefaultPages_ShouldBeRegistered()
    {
        // Act & Assert
        Assert.NotNull(_navigationService.CurrentPageKey);
    }

    [Fact]
    public async Task NavigateAsync_WithValidPageKey_ShouldRaiseNavigationEvent()
    {
        // Arrange
        _navigationEventRaised = false;

        // Act
        await _navigationService.NavigateAsync("Analyze");

        // Assert
        Assert.True(_navigationEventRaised);
        Assert.Equal("Analyze", _navigationService.CurrentPageKey);
    }

    [Fact]
    public async Task NavigateAsync_WithInvalidPageKey_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _navigationService.NavigateAsync("InvalidPage"));
    }

    [Fact]
    public async Task NavigateAsync_Generic_ShouldWork()
    {
        // Arrange
        _navigationEventRaised = false;

        // Act
        await _navigationService.NavigateAsync<AnalyzeViewModel>();

        // Assert
        Assert.True(_navigationEventRaised);
    }

    [Fact]
    public async Task GoBackAsync_WithHistory_ShouldNavigateBack()
    {
        // Arrange
        await _navigationService.NavigateAsync("Analyze");
        await _navigationService.NavigateAsync("DataCluster");

        // Act
        var result = await _navigationService.GoBackAsync();

        // Assert
        Assert.True(result);
        Assert.Equal("Analyze", _navigationService.CurrentPageKey);
    }

    [Fact]
    public async Task GoBackAsync_WithoutHistory_ShouldReturnFalse()
    {
        // Act
        var result = await _navigationService.GoBackAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GoForwardAsync_WithForwardHistory_ShouldNavigateForward()
    {
        // Arrange
        await _navigationService.NavigateAsync("Analyze");
        await _navigationService.NavigateAsync("DataCluster");
        await _navigationService.GoBackAsync(); // Go back to create forward history

        // Act
        var result = await _navigationService.GoForwardAsync();

        // Assert
        Assert.True(result);
        Assert.Equal("DataCluster", _navigationService.CurrentPageKey);
    }

    [Fact]
    public async Task GoForwardAsync_WithoutForwardHistory_ShouldReturnFalse()
    {
        // Act
        var result = await _navigationService.GoForwardAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanGoBack_ShouldReflectHistoryState()
    {
        // Arrange & Act
        Assert.False(_navigationService.CanGoBack);
        
        await _navigationService.NavigateAsync("Analyze");
        await _navigationService.NavigateAsync("DataCluster");
        
        // Assert
        Assert.True(_navigationService.CanGoBack);
    }

    [Fact]
    public async Task CanGoForward_ShouldReflectForwardHistoryState()
    {
        // Arrange & Act
        Assert.False(_navigationService.CanGoForward);
        
        await _navigationService.NavigateAsync("Analyze");
        await _navigationService.NavigateAsync("DataCluster");
        await _navigationService.GoBackAsync();
        
        // Assert
        Assert.True(_navigationService.CanGoForward);
    }

    [Fact]
    public async Task ClearHistory_ShouldRemoveAllHistory()
    {
        // Arrange
        await _navigationService.NavigateAsync("Analyze");
        await _navigationService.NavigateAsync("DataCluster");

        // Act
        _navigationService.ClearHistory();

        // Assert
        Assert.False(_navigationService.CanGoBack);
        Assert.False(_navigationService.CanGoForward);
    }

    [Fact]
    public void RegisterPage_ShouldAddPageToRegistry()
    {
        // Act
        _navigationService.RegisterPage("CustomPage", typeof(AnalyzeViewModel));

        // Assert - Should not throw when navigating to the registered page
        var exception = Record.ExceptionAsync(() => _navigationService.NavigateAsync("CustomPage"));
        Assert.Null(exception.Result);
    }
}