using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    private readonly AnalyzeViewModel _analyzeViewModel;
    private readonly DataClusterViewModel _dataClusterViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    public ShellViewModel(
        AnalyzeViewModel analyzeViewModel,
        DataClusterViewModel dataClusterViewModel,
        SettingsViewModel settingsViewModel)
    {
        _analyzeViewModel = analyzeViewModel;
        _dataClusterViewModel = dataClusterViewModel;
        _settingsViewModel = settingsViewModel;

        // Set the default page
        NavigateToAnalyze();
    }

    [RelayCommand]
    private void NavigateToAnalyze()
    {
        CurrentPage = _analyzeViewModel;
        CurrentPageTitle = "Analyze";
        CurrentPageSubtitle = "Face Detection & Analysis";
        
        // Update button states
        AnalyzeButtonBackground = "#007acc";
        DataClusterButtonBackground = "#333333";
        SettingsButtonBackground = "#333333";
    }

    [RelayCommand]
    private void NavigateToDataCluster()
    {
        CurrentPage = _dataClusterViewModel;
        CurrentPageTitle = "Data Cluster";
        CurrentPageSubtitle = "Metadata Analysis & Clustering";
        
        // Update button states
        AnalyzeButtonBackground = "#333333";
        DataClusterButtonBackground = "#007acc";
        SettingsButtonBackground = "#333333";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentPage = _settingsViewModel;
        CurrentPageTitle = "Settings";
        CurrentPageSubtitle = "Application Configuration";
        
        // Update button states
        AnalyzeButtonBackground = "#333333";
        DataClusterButtonBackground = "#333333";
        SettingsButtonBackground = "#007acc";
    }
}
