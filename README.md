# Ductilator Cross Platform

A cross-platform HVAC ductwork calculator built with Avalonia UI and .NET 6.0.

## Description

Ductilator is a professional tool for calculating air duct dimensions and properties for HVAC systems. The application performs calculations based on ASHRAE (American Society of Heating, Refrigerating and Air-Conditioning Engineers) standards and methodologies.

## Features

- Cross-platform compatibility (Windows, macOS, Linux)
- Modern UI built with Avalonia
- HVAC duct calculations following ASHRAE standards
- Real-time calculation updates
- Support for multiple fluid properties and parameters

## Requirements

- .NET 6.0 SDK or later
- Works on Windows, macOS, and Linux

## Building the Project

```bash
# Clone the repository
git clone https://github.com/robystarboy/Ductilator_Cross_Platform.git
cd Ductilator_Cross_Platform

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

## Development

This project can be opened and developed using:
- Visual Studio Code
- JetBrains Rider
- Visual Studio 2022

## Project Structure

```
Ductilator_Cross_Platform/
├── Assets/              # Application assets
├── Behaviors/           # Custom Avalonia behaviors
├── Converters/          # Value converters for data binding
├── Models/              # Data models
├── Services/            # Business logic and calculation services
├── ViewModels/          # MVVM view models
├── Views/               # Additional views
├── MainWindow.axaml     # Main application window
└── App.axaml            # Application entry point
```

## Technologies Used

- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML-based UI framework
- .NET 6.0
- MVVM (Model-View-ViewModel) architecture

## License

[Choose an appropriate license - MIT, Apache 2.0, GPL, etc.]

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Acknowledgments

Calculations based on ASHRAE Handbook - Fundamentals standards.

