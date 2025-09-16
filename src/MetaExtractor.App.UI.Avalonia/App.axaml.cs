using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MetaExtractor.App.UI.Avalonia.Views;
using MetaExtractor.Core.Interfaces;
using MetaExtractor.Core.Services;
using MetaExtractor.Core.ViewModels;
using MetaExtractor.Infrastructure.Data;
using MetaExtractor.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MetaExtractor.App.UI.Avalonia;

public partial class App : Application
{
    public IHost? AppHost { get; private set; }

    // A public property to access the service provider
    public IServiceProvider Services => AppHost!.Services;

    // A static property to access the current App instance
    public new static App Current => (App)Application.Current!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureServices(services);
            })
            .Build();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database Configuration
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite("Data Source=metaextractor.db");
        });

        // Core Services
        services.AddScoped<IFaceDetectionService, OpenCvFaceDetectionService>();
        services.AddScoped<IImageProcessingService, ImageProcessingService>();
        
        // Image Source Strategies
        services.AddTransient<CameraImageSourceStrategy>();
        services.AddTransient<FileImageSourceStrategy>();
        
        // Default strategy registration
        services.AddTransient<IImageSourceStrategy, CameraImageSourceStrategy>();
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels with proper dependency injection
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<AnalyzeViewModel>();
        services.AddTransient<DataClusterViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views (UI Components)
        // Shell as singleton for application lifetime, pages as transient for navigation
        services.AddSingleton<ShellView>();
        services.AddTransient<AnalyzeView>();
        services.AddTransient<DataClusterView>();
        services.AddTransient<SettingsView>();

        // Configuration Services
        // TODO: Add INavigationService when implemented
        // TODO: Add configuration services for settings management
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<ShellView>();
            desktop.MainWindow.DataContext = Services.GetRequiredService<ShellViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
