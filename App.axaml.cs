using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace Ductilator;

public partial class App : Application
{
    public override void Initialize()
    {
        try
        {
            Console.WriteLine("App: Initialize started");
            AvaloniaXamlLoader.Load(this);
            Console.WriteLine("App: Initialize completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in App.Initialize: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            Console.WriteLine("App: OnFrameworkInitializationCompleted started");
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("App: Creating MainWindow...");
                desktop.MainWindow = new MainWindow();
                Console.WriteLine("App: MainWindow created and assigned");
            }
            else
            {
                Console.WriteLine("WARNING: ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime");
            }

            base.OnFrameworkInitializationCompleted();
            Console.WriteLine("App: OnFrameworkInitializationCompleted completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in App.OnFrameworkInitializationCompleted: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}