using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Ductilator.Models;

/// <summary>
/// Represents a fluid property with Imperial and Metric values
/// Implements INotifyPropertyChanged for data binding support
/// </summary>
public class FluidProperty : INotifyPropertyChanged
{
    private string _name;
    private double _imperialValue;
    private string _imperialUnit;
    private double _metricValue;
    private string _metricUnit;
    private double _conversionFactor;
    private bool _isUpdating;
    private string _imperialValueText = string.Empty;
    private string _metricValueText = string.Empty;
    private bool _isTextInput;

    /// <summary>
    /// Gets or sets the property name (e.g., "Fluid Density")
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the conversion factor (Imperial to Metric)
    /// </summary>
    public double ConversionFactor
    {
        get => _conversionFactor;
        set => SetProperty(ref _conversionFactor, value);
    }

    /// <summary>
    /// Gets or sets the imperial (IP) value
    /// </summary>
    public double ImperialValue
    {
        get => _imperialValue;
        set
        {
            if (SetProperty(ref _imperialValue, value))
            {
                if (!_isTextInput)
                {
                    _imperialValueText = FormatToFourDecimals(_imperialValue);
                }
                OnPropertyChanged(nameof(ImperialValueText));
                if (!_isUpdating && _conversionFactor != 0)
                {
                    _isUpdating = true;
                    MetricValue = _imperialValue * _conversionFactor;
                    _isUpdating = false;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the imperial value as formatted text (4 decimal places)
    /// </summary>
    public string ImperialValueText
    {
        get => _imperialValueText;
        set
        {
            _imperialValueText = value;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
            {
                _isTextInput = true;
                ImperialValue = parsed;
                _isTextInput = false;
            }
        }
    }

    /// <summary>
    /// Gets or sets the imperial unit (e.g., "lb/ft³")
    /// </summary>
    public string ImperialUnit
    {
        get => _imperialUnit;
        set => SetProperty(ref _imperialUnit, value);
    }

    /// <summary>
    /// Gets or sets the metric (SI) value
    /// </summary>
    public double MetricValue
    {
        get => _metricValue;
        set
        {
            if (SetProperty(ref _metricValue, value))
            {
                if (!_isTextInput)
                {
                    _metricValueText = FormatToFourDecimals(_metricValue);
                }
                OnPropertyChanged(nameof(MetricValueText));
                if (!_isUpdating && _conversionFactor != 0)
                {
                    _isUpdating = true;
                    ImperialValue = _metricValue / _conversionFactor;
                    _isUpdating = false;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the metric value as formatted text (4 decimal places)
    /// </summary>
    public string MetricValueText
    {
        get => _metricValueText;
        set
        {
            _metricValueText = value;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
            {
                _isTextInput = true;
                MetricValue = parsed;
                _isTextInput = false;
            }
        }
    }

    /// <summary>
    /// Gets or sets the metric unit (e.g., "kg/m³")
    /// </summary>
    public string MetricUnit
    {
        get => _metricUnit;
        set => SetProperty(ref _metricUnit, value);
    }

    /// <summary>
    /// Format a number to 4 decimal places
    /// </summary>
    private string FormatToFourDecimals(double value)
    {
        return value.ToString("F4", CultureInfo.InvariantCulture);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Helper method to raise PropertyChanged event with compiler support
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
