using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ductilator.Models;

/// <summary>
/// Represents a duct parameter with value and calculated result
/// Implements INotifyPropertyChanged for data binding support
/// </summary>
public class DuctParameter : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private double _imperialValue;
    private double _metricValue;
    private string _imperialUnit = string.Empty;
    private string _metricUnit = string.Empty;
    private double _conversionFactor;
    private bool _isUpdating;
    private bool _isReadOnly;
    private bool _isLocked;
    private string _imperialValueText = string.Empty;
    private string _metricValueText = string.Empty;
    private bool _isTextInput;

    /// <summary>
    /// Gets or sets the parameter name (e.g., "Duct Diameter")
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the input value in imperial units
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
    /// Gets the formatted imperial value as a string (4 decimal places)
    /// </summary>
    public string ImperialValueText
    {
        get => _imperialValueText;
        set
        {
            _imperialValueText = value;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                _isTextInput = true;
                ImperialValue = parsed;
                _isTextInput = false;
            }
        }
    }

    /// <summary>
    /// Gets or sets the unit of measurement for imperial values
    /// </summary>
    public string ImperialUnit
    {
        get => _imperialUnit;
        set => SetProperty(ref _imperialUnit, value);
    }

    /// <summary>
    /// Gets or sets the input value in metric units
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
    /// Gets the formatted metric value as a string (4 decimal places)
    /// </summary>
    public string MetricValueText
    {
        get => _metricValueText;
        set
        {
            _metricValueText = value;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                _isTextInput = true;
                MetricValue = parsed;
                _isTextInput = false;
            }
        }
    }

    /// <summary>
    /// Gets or sets the unit of measurement for metric values
    /// </summary>
    public string MetricUnit
    {
        get => _metricUnit;
        set => SetProperty(ref _metricUnit, value);
    }

    /// <summary>
    /// Gets or sets the conversion factor between imperial and metric values
    /// </summary>
    public double ConversionFactor
    {
        get => _conversionFactor;
        set => SetProperty(ref _conversionFactor, value);
    }

    /// <summary>
    /// Gets or sets whether this parameter is read-only
    /// </summary>
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetProperty(ref _isReadOnly, value);
    }

    /// <summary>
    /// Gets or sets whether this parameter value is locked (prevents recalculation)
    /// </summary>
    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (SetProperty(ref _isLocked, value))
            {
                // Notify that IsEffectivelyReadOnly may have changed
                OnPropertyChanged(nameof(IsEffectivelyReadOnly));
            }
        }
    }

    /// <summary>
    /// Gets whether this parameter is effectively read-only (either IsReadOnly or IsLocked)
    /// </summary>
    public bool IsEffectivelyReadOnly => _isReadOnly || _isLocked;

    /// <summary>
    /// Sets initial imperial and metric values without applying conversions.
    /// </summary>
    public void SetDefaults(double imperialValue, double metricValue)
    {
        _isUpdating = true;

        if (SetProperty(ref _imperialValue, imperialValue, nameof(ImperialValue)))
        {
            _imperialValueText = FormatToFourDecimals(_imperialValue);
            OnPropertyChanged(nameof(ImperialValueText));
        }

        if (SetProperty(ref _metricValue, metricValue, nameof(MetricValue)))
        {
            _metricValueText = FormatToFourDecimals(_metricValue);
            OnPropertyChanged(nameof(MetricValueText));
        }

        _isUpdating = false;
    }

    /// <summary>
    /// Gets or sets the value, defaulting to the imperial value
    /// </summary>
    public double Value
    {
        get => ImperialValue;
        set => ImperialValue = value;
    }

    /// <summary>
    /// Gets or sets the unit, defaulting to the imperial unit
    /// </summary>
    public string Unit
    {
        get => ImperialUnit;
        set => ImperialUnit = value;
    }

    /// <summary>
    /// Gets or sets the result value, defaulting to the metric value
    /// </summary>
    public double Result
    {
        get => MetricValue;
        set => MetricValue = value;
    }

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

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    protected void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}