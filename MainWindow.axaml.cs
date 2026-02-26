using Avalonia.Controls;
using Avalonia.Interactivity;
using Ductilator.ViewModels;
using System;

namespace Ductilator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            Console.WriteLine("MainWindow: Constructor started");
            
            InitializeComponent();
            Console.WriteLine("MainWindow: InitializeComponent completed");
            
            DataContext = new MainViewModel();
            Console.WriteLine("MainWindow: DataContext set");
            
            // Subscribe to window size changed event
            PropertyChanged += OnWindowPropertyChanged;
            
            // Set initial window size
            if (DataContext is MainViewModel vm)
            {
                vm.WindowWidth = ClientSize.Width;
                vm.WindowHeight = ClientSize.Height;
            }
            
            Console.WriteLine("MainWindow: Constructor completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in MainWindow constructor: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
            }
            throw; // Re-throw to see full error
        }
    }

    // Event handler for window property changes (including size)
    private void OnWindowPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ClientSizeProperty && DataContext is MainViewModel vm)
        {
            vm.WindowWidth = ClientSize.Width;
            vm.WindowHeight = ClientSize.Height;
        }
    }

    // Event handlers for menu items and other interactions

    private void OnFileExit(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnViewRefresh(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.RefreshData();
        }
    }

    private void OnLoadAirProperties(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.LoadAirProperties();
        }
    }

    private void OnLoadWaterProperties(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.LoadWaterProperties();
        }
    }

    private void OnCustomFluid(object sender, RoutedEventArgs e)
    {
        // TODO: Implement custom fluid dialog
    }

    private void OnHelpAbout(object sender, RoutedEventArgs e)
    {
        // TODO: Implement about dialog
    }
}