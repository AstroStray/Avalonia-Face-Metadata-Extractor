using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaExtractor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MetaExtractor.Core.Services;

/// <summary>
/// Implementation of navigation service for page routing.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _pageRegistry;
    private readonly Stack<NavigationEntry> _backStack;
    private readonly Stack<NavigationEntry> _forwardStack;
    private string? _currentPageKey;

    public event EventHandler<NavigationEventArgs>? NavigationRequested;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _pageRegistry = new Dictionary<string, Type>();
        _backStack = new Stack<NavigationEntry>();
        _forwardStack = new Stack<NavigationEntry>();

        RegisterDefaultPages();
    }

    public bool CanGoBack => _backStack.Count > 0;

    public bool CanGoForward => _forwardStack.Count > 0;

    public string? CurrentPageKey => _currentPageKey;

    public async Task NavigateAsync(string pageKey, IDictionary<string, object>? parameters = null)
    {
        if (!_pageRegistry.ContainsKey(pageKey))
        {
            throw new InvalidOperationException($"Page '{pageKey}' is not registered in the navigation service.");
        }

        var viewModelType = _pageRegistry[pageKey];
        var viewModel = _serviceProvider.GetRequiredService(viewModelType);

        await PerformNavigationAsync(pageKey, viewModel, parameters);
    }

    public async Task NavigateAsync<TViewModel>(IDictionary<string, object>? parameters = null)
    {
        var viewModelType = typeof(TViewModel);
        var pageKey = _pageRegistry.FirstOrDefault(kvp => kvp.Value == viewModelType).Key;

        if (string.IsNullOrEmpty(pageKey))
        {
            pageKey = viewModelType.Name;
            RegisterPage(pageKey, viewModelType);
        }

        await NavigateAsync(pageKey, parameters);
    }

    public async Task<bool> GoBackAsync()
    {
        if (!CanGoBack)
            return false;

        var previousEntry = _backStack.Pop();

        // Push current page to forward stack
        if (_currentPageKey != null)
        {
            _forwardStack.Push(new NavigationEntry(_currentPageKey, null)); // We don't store ViewModels in history for now
        }

        await PerformNavigationAsync(previousEntry.PageKey, null, previousEntry.Parameters, addToHistory: false);
        return true;
    }

    public async Task<bool> GoForwardAsync()
    {
        if (!CanGoForward)
            return false;

        var nextEntry = _forwardStack.Pop();

        // Push current page to back stack
        if (_currentPageKey != null)
        {
            _backStack.Push(new NavigationEntry(_currentPageKey, null));
        }

        await PerformNavigationAsync(nextEntry.PageKey, null, nextEntry.Parameters, addToHistory: false);
        return true;
    }

    public void ClearHistory()
    {
        _backStack.Clear();
        _forwardStack.Clear();
    }

    /// <summary>
    /// Register a page with the navigation service.
    /// </summary>
    /// <param name="pageKey">The page key.</param>
    /// <param name="viewModelType">The ViewModel type.</param>
    public void RegisterPage(string pageKey, Type viewModelType)
    {
        _pageRegistry[pageKey] = viewModelType;
    }

    private void RegisterDefaultPages()
    {
        RegisterPage("Analyze", typeof(AnalyzeViewModel));
        RegisterPage("DataCluster", typeof(DataClusterViewModel));
        RegisterPage("Settings", typeof(SettingsViewModel));
    }

    private async Task PerformNavigationAsync(string pageKey, object? viewModel, IDictionary<string, object>? parameters, bool addToHistory = true)
    {
        // Add current page to back stack if navigating forward
        if (addToHistory && _currentPageKey != null)
        {
            _backStack.Push(new NavigationEntry(_currentPageKey, null));
            _forwardStack.Clear(); // Clear forward stack on new navigation
        }

        // If viewModel is null, create it from the service provider
        if (viewModel == null && _pageRegistry.ContainsKey(pageKey))
        {
            viewModel = _serviceProvider.GetRequiredService(_pageRegistry[pageKey]);
        }

        // Pass parameters to ViewModel if it supports them
        if (parameters != null && viewModel is INavigationAware navigationAwareViewModel)
        {
            await navigationAwareViewModel.OnNavigatedToAsync(parameters);
        }

        _currentPageKey = pageKey;

        // Raise navigation event
        NavigationRequested?.Invoke(this, new NavigationEventArgs(pageKey, viewModel, parameters));
    }

    private record NavigationEntry(string PageKey, IDictionary<string, object>? Parameters);
}

/// <summary>
/// Interface for ViewModels that need to be notified of navigation events.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Called when navigating to this ViewModel.
    /// </summary>
    /// <param name="parameters">Navigation parameters.</param>
    Task OnNavigatedToAsync(IDictionary<string, object> parameters);

    /// <summary>
    /// Called when navigating away from this ViewModel.
    /// </summary>
    Task OnNavigatedFromAsync();
}