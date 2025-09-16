using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetaExtractor.Core.Services;

namespace MetaExtractor.Core.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    // This will hold the view model for the currently displayed page.
    [ObservableProperty]
    private ObservableObject? _currentPage;

    // Navigation state properties
    [ObservableProperty]
    private string _currentPageTitle = "Analyze";

    [ObservableProperty]
    private string _currentPageSubtitle = "Face Detection & Analysis";

    // Button background properties for active state
    [ObservableProperty]
    private string _analyzeButtonBackground = "#007acc";

    [ObservableProperty]
    private string _dataClusterButtonBackground = "#333333";

    [ObservableProperty]
    private string _settingsButtonBackground = "#333333";

    private readonly INavigationService _navigationService;

    public ShellViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        // Subscribe to navigation events
        _navigationService.NavigationRequested += OnNavigationRequested;

        // Set the default page
        NavigateToAnalyze();
    }

    private void OnNavigationRequested(object? sender, NavigationEventArgs e)
    {
        CurrentPage = e.ViewModel as ObservableObject;
        
        // Update UI based on the page key
        switch (e.PageKey)
        {
            case "Analyze":
                CurrentPageTitle = "Analyze";
                CurrentPageSubtitle = "Face Detection & Analysis";
                AnalyzeButtonBackground = "#007acc";
                DataClusterButtonBackground = "#333333";
                SettingsButtonBackground = "#333333";
                break;
            case "DataCluster":
                CurrentPageTitle = "Data Cluster";
                CurrentPageSubtitle = "Metadata Analysis & Clustering";
                AnalyzeButtonBackground = "#333333";
                DataClusterButtonBackground = "#007acc";
                SettingsButtonBackground = "#333333";
                break;
            case "Settings":
                CurrentPageTitle = "Settings";
                CurrentPageSubtitle = "Application Configuration";
                AnalyzeButtonBackground = "#333333";
                DataClusterButtonBackground = "#333333";
                SettingsButtonBackground = "#007acc";
                break;
        }
    }

    [RelayCommand]
    private async Task NavigateToAnalyze()
    {
        await _navigationService.NavigateAsync("Analyze");
    }

    [RelayCommand]
    private async Task NavigateToDataCluster()
    {
        await _navigationService.NavigateAsync("DataCluster");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await _navigationService.NavigateAsync("Settings");
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    private async Task GoForward()
    {
        await _navigationService.GoForwardAsync();
    }

    public bool CanGoBack => _navigationService.CanGoBack;
    public bool CanGoForward => _navigationService.CanGoForward;
}
