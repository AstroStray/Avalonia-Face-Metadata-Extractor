using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace MetaExtractor.App.UI.Avalonia;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "DataContext is null" };
        }

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        // Adjust the namespace for Views. They are in MetaExtractor.App.UI.Avalonia.Views
        name = name.Replace("MetaExtractor.Core.ViewModels", "MetaExtractor.App.UI.Avalonia.Views");
        var type = Type.GetType(name);

        if (type != null)
        {
            // Get the service provider from the App class
            var sp = (App.Current as App)!.Services;
            var control = (Control)sp.GetRequiredService(type);
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        // We match any view model that inherits from ObservableObject
        return data is ObservableObject;
    }
}
