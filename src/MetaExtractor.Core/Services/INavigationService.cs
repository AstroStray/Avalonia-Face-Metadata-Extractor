using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaExtractor.Core.Services;

/// <summary>
/// Defines navigation functionality for the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Event raised when navigation occurs.
    /// </summary>
    event EventHandler<NavigationEventArgs> NavigationRequested;

    /// <summary>
    /// Navigate to a specific page by its key.
    /// </summary>
    /// <param name="pageKey">The key identifying the target page.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <returns>A task representing the async operation.</returns>
    Task NavigateAsync(string pageKey, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Navigate to a specific page by its type.
    /// </summary>
    /// <typeparam name="TViewModel">The target ViewModel type.</typeparam>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <returns>A task representing the async operation.</returns>
    Task NavigateAsync<TViewModel>(IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Navigate back to the previous page if possible.
    /// </summary>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    Task<bool> GoBackAsync();

    /// <summary>
    /// Navigate forward to the next page if possible.
    /// </summary>
    /// <returns>True if navigation was successful, false otherwise.</returns>
    Task<bool> GoForwardAsync();

    /// <summary>
    /// Check if backward navigation is possible.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Check if forward navigation is possible.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Get the current page key.
    /// </summary>
    string? CurrentPageKey { get; }

    /// <summary>
    /// Clear the navigation history.
    /// </summary>
    void ClearHistory();
}

/// <summary>
/// Event args for navigation events.
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public string PageKey { get; }
    public IDictionary<string, object>? Parameters { get; }
    public object? ViewModel { get; }

    public NavigationEventArgs(string pageKey, object? viewModel = null, IDictionary<string, object>? parameters = null)
    {
        PageKey = pageKey;
        ViewModel = viewModel;
        Parameters = parameters;
    }
}