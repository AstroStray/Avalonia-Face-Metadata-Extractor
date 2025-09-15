using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MetaExtractor.Core.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    // This will hold the view model for the currently displayed page.
    [ObservableProperty]
    private ObservableObject? _currentPage;

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
        CurrentPage = _analyzeViewModel;
    }

    [RelayCommand]
    private void NavigateToAnalyze()
    {
        CurrentPage = _analyzeViewModel;
    }

    [RelayCommand]
    private void NavigateToDataCluster()
    {
        CurrentPage = _dataClusterViewModel;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentPage = _settingsViewModel;
    }
}
