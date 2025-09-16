using MetaExtractor.Core.Interfaces;
using MetaExtractor.Core.Services;
using MetaExtractor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MetaExtractor.Core.Tests.DependencyInjection;

public class ServiceRegistrationTests
{
    private IServiceCollection CreateMinimalServiceCollection()
    {
        var services = new ServiceCollection();
        
        // Register core services with mocks for dependencies not in Core layer
        services.AddScoped<IFaceDetectionService, OpenCvFaceDetectionService>();
        services.AddScoped<IImageProcessingService, ImageProcessingService>();
        
        // Register ViewModels
        services.AddTransient<AnalyzeViewModel>();
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<DataClusterViewModel>();
        services.AddTransient<SettingsViewModel>();

        return services;
    }

    [Fact]
    public void ServiceCollection_ShouldRegisterCoreServices()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        
        // Act
        var serviceDescriptors = services.ToList();
        
        // Assert
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(IFaceDetectionService));
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(IImageProcessingService));
    }

    [Fact]
    public void ServiceCollection_ShouldRegisterAllViewModels()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        
        // Act
        var serviceDescriptors = services.ToList();
        
        // Assert
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(AnalyzeViewModel));
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(ShellViewModel));
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(DataClusterViewModel));
        Assert.Contains(serviceDescriptors, s => s.ServiceType == typeof(SettingsViewModel));
    }

    [Fact]
    public void ServiceLifetimes_ShouldBeConfiguredCorrectly()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        var serviceDescriptors = services.ToList();
        
        // Act & Assert - Check service lifetimes
        var faceDetectionService = serviceDescriptors.Find(s => s.ServiceType == typeof(IFaceDetectionService));
        Assert.Equal(ServiceLifetime.Scoped, faceDetectionService?.Lifetime);

        var imageProcessingService = serviceDescriptors.Find(s => s.ServiceType == typeof(IImageProcessingService));
        Assert.Equal(ServiceLifetime.Scoped, imageProcessingService?.Lifetime);

        var shellViewModel = serviceDescriptors.Find(s => s.ServiceType == typeof(ShellViewModel));
        Assert.Equal(ServiceLifetime.Singleton, shellViewModel?.Lifetime);

        var analyzeViewModel = serviceDescriptors.Find(s => s.ServiceType == typeof(AnalyzeViewModel));
        Assert.Equal(ServiceLifetime.Transient, analyzeViewModel?.Lifetime);
    }

    [Fact]
    public void DependencyInjection_ShouldHandleComplexDependencies()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();

        // Build service provider
        var provider = services.BuildServiceProvider();

        // Act & Assert - Should not throw when resolving services with dependencies
        var exception = Record.Exception(() =>
        {
            var analyzeViewModel = provider.GetService<AnalyzeViewModel>();
            Assert.NotNull(analyzeViewModel);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ServiceProvider_ShouldCreateDifferentTransientInstances()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var analyzeViewModel1 = provider.GetService<AnalyzeViewModel>();
        var analyzeViewModel2 = provider.GetService<AnalyzeViewModel>();

        // Assert
        Assert.NotNull(analyzeViewModel1);
        Assert.NotNull(analyzeViewModel2);
        Assert.NotSame(analyzeViewModel1, analyzeViewModel2);
    }

    [Fact]
    public void ServiceProvider_ShouldReturnSameSingletonInstance()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var shellViewModel1 = provider.GetService<ShellViewModel>();
        var shellViewModel2 = provider.GetService<ShellViewModel>();

        // Assert
        Assert.NotNull(shellViewModel1);
        Assert.NotNull(shellViewModel2);
        Assert.Same(shellViewModel1, shellViewModel2);
    }

    [Fact]
    public void ServiceProvider_ShouldReturnSameScopedInstanceWithinScope()
    {
        // Arrange
        var services = CreateMinimalServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act & Assert
        using var scope = provider.CreateScope();
        var faceDetectionService1 = scope.ServiceProvider.GetService<IFaceDetectionService>();
        var faceDetectionService2 = scope.ServiceProvider.GetService<IFaceDetectionService>();

        Assert.NotNull(faceDetectionService1);
        Assert.NotNull(faceDetectionService2);
        Assert.Same(faceDetectionService1, faceDetectionService2);
    }
}