using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ductilator.Models;
using Ductilator.Services;

namespace Ductilator.ViewModels;

/// <summary>
/// Main ViewModel for the Ductilator application
/// Manages data binding and application logic
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "Ready";
    private string _versionInfo = "Version: 2026.058.0.002";
    private int _selectedConditionIndex = 0;
    private bool _isInitializing = true;
    private bool _isCalculating = false;
    private double _windowWidth = 1000;
    private double _windowHeight = 775;
    private readonly CalculationService _calculationService;

    public ObservableCollection<DuctParameter> DuctParameters { get; }
    public ObservableCollection<FluidProperty> FluidProperties { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string VersionInfo
    {
        get => _versionInfo;
        set => SetProperty(ref _versionInfo, value);
    }

    public double WindowWidth
    {
        get => _windowWidth;
        set
        {
            if (SetProperty(ref _windowWidth, value))
            {
                OnPropertyChanged(nameof(WindowSizeText));
            }
        }
    }

    public double WindowHeight
    {
        get => _windowHeight;
        set
        {
            if (SetProperty(ref _windowHeight, value))
            {
                OnPropertyChanged(nameof(WindowSizeText));
            }
        }
    }

    public string WindowSizeText => $"Width: {WindowWidth:F0} × Height: {WindowHeight:F0}";

    public int SelectedConditionIndex
    {
        get => _selectedConditionIndex;
        set
        {
            if (SetProperty(ref _selectedConditionIndex, value))
            {
                Console.WriteLine($"MainViewModel: SelectedConditionIndex changed to {value}");
                // Only automatically load if not during initialization
                if (!_isInitializing)
                {
                    Console.WriteLine($"MainViewModel: Loading standard air properties for condition {value}");
                    LoadStandardAirProperties(value);
                }
                else
                {
                    Console.WriteLine("MainViewModel: Skipping auto-load during initialization");
                }
            }
        }
    }

    public MainViewModel()
    {
        try
        {
            Console.WriteLine("MainViewModel: Constructor started");
            
            _calculationService = new CalculationService();
            Console.WriteLine("MainViewModel: CalculationService created");
            
            DuctParameters = new ObservableCollection<DuctParameter>();
            FluidProperties = new ObservableCollection<FluidProperty>();
            Console.WriteLine("MainViewModel: Collections initialized");

            InitializeData();
            Console.WriteLine("MainViewModel: InitializeData completed");
            
            // Initialization complete, now property changes will trigger loading
            _isInitializing = false;
            Console.WriteLine("MainViewModel: Constructor completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in MainViewModel constructor: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to prevent app from running in broken state
        }
    }

    /// <summary>
    /// Initialize UI with default data
    /// </summary>
    private void InitializeData()
    {
        const double cfmToLs = 0.47194745;
        const double fpmToMs = 0.3048 / 60.0;
        const double inToMm = 25.4;
        const double inWcToPa = 248.84;
        const double inWcPer100FtToPaPerM = 8.1726;

        DuctParameter CreateDuctParameter(
            string name,
            string imperialUnit,
            string metricUnit,
            double conversionFactor,
            bool isReadOnly,
            double imperialValue,
            double metricValue)
        {
            var parameter = new DuctParameter
            {
                Name = name,
                ImperialUnit = imperialUnit,
                MetricUnit = metricUnit,
                ConversionFactor = conversionFactor,
                IsReadOnly = isReadOnly
            };

            parameter.SetDefaults(imperialValue, metricValue);
            
            // Subscribe to property changes for editable parameters
            if (!isReadOnly)
            {
                parameter.PropertyChanged += OnDuctParameterChanged;
            }
            
            // Subscribe to IsLocked changes for all editable parameters
            if (!isReadOnly)
            {
                parameter.PropertyChanged += OnParameterLockedChanged;
            }
            
            return parameter;
        }

        DuctParameters.Add(CreateDuctParameter("Flow Rate", "ft³/min", "L/s", cfmToLs, false, 500.0, 236.0));
        DuctParameters.Add(CreateDuctParameter("Head Loss", "in WC/100 ft", "Pa/m", inWcPer100FtToPaPerM, false, 0.080, 0.653));
        DuctParameters.Add(CreateDuctParameter("Fluid Velocity", "fpm", "m/s", fpmToMs, false, 732.5, 3.712));
        DuctParameters.Add(CreateDuctParameter("Equivalent Diameter", "in", "mm", inToMm, false, 11.2, 284.2));
        DuctParameters.Add(CreateDuctParameter("Duct Size X", "in", "mm", inToMm, false, 10.0, 250.0));
        DuctParameters.Add(CreateDuctParameter("Duct Size Y", "in", "mm", inToMm, false, 10.0, 275.0));
        DuctParameters.Add(CreateDuctParameter("Equivalent Diameter", "in", "mm", inToMm, true, 10.93, 286.55));
        DuctParameters.Add(CreateDuctParameter("Flow Area", "ft²", "m²", 0.092903, true, 0.6518, 0.0645));
        DuctParameters.Add(CreateDuctParameter("Fluid Velocity", "fpm", "m/s", fpmToMs, true, 767.1, 3.659));
        DuctParameters.Add(CreateDuctParameter("Reynolds Number", "", "", 1.0, true, 72793.0, 70534.0));
        DuctParameters.Add(CreateDuctParameter("Friction Factor", "", "", 1.0, true, 0.02218, 0.02219));
        DuctParameters.Add(CreateDuctParameter("Velocity Pressure", "in WC", "Pa", inWcToPa, true, 0.0367, 8.044));
        DuctParameters.Add(CreateDuctParameter("Head Loss", "in WC", "Pa", inWcToPa, true, 0.089, 0.624));

        // Load default fluid properties (68°F/20°C Air @ STP)
        LoadStandardAirProperties(0);

        StatusMessage = "Application initialized";
    }

    /// <summary>
    /// Load standard air properties
    /// </summary>
    public void LoadAirProperties()
    {
        FluidProperties.Clear();
        
        // Fluid Density: lb/ft³ to kg/m³ (multiply by 16.0185)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Fluid Density", 
            ImperialValue = 0.075, 
            ImperialUnit = "lb/ft³", 
            MetricValue = 1.2014, 
            MetricUnit = "kg/m³",
            ConversionFactor = 16.0185
        });
        
        // Fluid Viscosity: lb/ft·h to kg/m·h (multiply by 1.4882)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Fluid Viscosity", 
            ImperialValue = 0.0473, 
            ImperialUnit = "lb/ft·h", 
            MetricValue = 0.0705, 
            MetricUnit = "kg/m·h",
            ConversionFactor = 1.4882
        });
        
        // Specific Heat: Btu/lb·°F to kJ/kg·°C (multiply by 4.1868)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Specific Heat", 
            ImperialValue = 0.24, 
            ImperialUnit = "Btu/lb·°F", 
            MetricValue = 1.0048, 
            MetricUnit = "kJ/kg·°C",
            ConversionFactor = 4.1868
        });
        
        // Energy Factor: Btu/h·°F·ft³/min to W/°C·L/s (multiply by 1.1204)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Energy Factor", 
            ImperialValue = 0.96, 
            ImperialUnit = "Btu/h·°F·ft³/min", 
            MetricValue = 1.08, 
            MetricUnit = "W/°C·L/s",
            ConversionFactor = 1.1204
        });

        StatusMessage = "Air properties loaded";
    }

    /// <summary>
    /// Load standard water properties
    /// </summary>
    public void LoadWaterProperties()
    {
        FluidProperties.Clear();
        
        // Fluid Density: lb/ft³ to kg/m³ (multiply by 16.0185)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Fluid Density", 
            ImperialValue = 62.3, 
            ImperialUnit = "lb/ft³", 
            MetricValue = 998.0, 
            MetricUnit = "kg/m³",
            ConversionFactor = 16.0185
        });
        
        // Fluid Velocity: lb/ft·h to kg/m·h (multiply by 1.48816)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Fluid Viscosity", 
            ImperialValue = 0.671, 
            ImperialUnit = "lb/ft·h", 
            MetricValue = 1.0, 
            MetricUnit = "kg/m·h",
            ConversionFactor = 1.48816
        });
        
        // Specific Heat: Btu/lb·°F to kJ/kg·°C (multiply by 4.1868)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Specific Heat", 
            ImperialValue = 1.0, 
            ImperialUnit = "Btu/lb·°F", 
            MetricValue = 4.1868, 
            MetricUnit = "kJ/kg·°C",
            ConversionFactor = 4.1868
        });
        
        // Energy Factor: Btu/h·°F·ft³/min to W/°C·L/s (multiply by 1.125)
        FluidProperties.Add(new FluidProperty 
        { 
            Name = "Energy Factor", 
            ImperialValue = 4.0, 
            ImperialUnit = "Btu/h·°F·ft³/min", 
            MetricValue = 4.5, 
            MetricUnit = "W/°C·L/s",
            ConversionFactor = 1.1204
        });

        StatusMessage = "Water properties loaded";
    }

    /// <summary>
    /// Refresh all data and recalculate
    /// </summary>
    public void RefreshData()
    {
        // TODO: Recalculate all parameters
        StatusMessage = "Data refreshed";
    }

    /// <summary>
    /// Load standard air properties based on temperature and humidity conditions
    /// </summary>
    /// <param name="condition">0=68°F STP, 1=55°F 97%RH, 2=75°F 50%RH, 3=100°F 23%RH, 4=125°F 11%RH</param>
    public void LoadStandardAirProperties(int condition)
    {
        Console.WriteLine($"MainViewModel: LoadStandardAirProperties called with condition={condition}");
        
        // Unsubscribe from existing fluid properties
        foreach (var prop in FluidProperties)
        {
            prop.PropertyChanged -= OnFluidPropertyChanged;
        }
        
        FluidProperties.Clear();
        Console.WriteLine($"MainViewModel: FluidProperties cleared, count={FluidProperties.Count}");
        
        double density_imp, density_met, velocity_imp, velocity_met, specificHeat_imp, specificHeat_met, energyFactor_imp, energyFactor_met;
        string conditionName;

        switch (condition)
        {
            case 0: // 68°F/20°C Air @ STP (Standard Temperature and Pressure)
                conditionName = "68°F/20°C Air @ STP";
                density_imp = 0.075;
                density_met = 1.2014;
                velocity_imp = 0.0473;
                velocity_met = 0.0705;
                specificHeat_imp = 0.24;
                specificHeat_met = 1.0048;
                energyFactor_imp = 0.96;
                energyFactor_met = 1.08;
                break;

            case 1: // 55°F/13°C Air @ 97% RH & 1 ATM (Cool, very humid)
                conditionName = "55°F/13°C Air @ 97% RH & 1 ATM";
                density_imp = 0.0765;
                density_met = 1.2254;
                velocity_imp = 0.0481;
                velocity_met = 0.0716;
                specificHeat_imp = 0.2405;
                specificHeat_met = 1.0069;
                energyFactor_imp = 0.978;
                energyFactor_met = 1.1003;
                break;

            case 2: // 75°F/25°C Air @ 50% RH & 1 ATM (Typical indoor comfort)
                conditionName = "75°F/25°C Air @ 50% RH & 1 ATM";
                density_imp = 0.0735;
                density_met = 1.1774;
                velocity_imp = 0.0463;
                velocity_met = 0.0689;
                specificHeat_imp = 0.2415;
                specificHeat_met = 1.0111;
                energyFactor_imp = 0.945;
                energyFactor_met = 1.0631;
                break;

            case 3: // 100°F/37°C Air @ 23% RH & 1 ATM (Hot, dry)
                conditionName = "100°F/37°C Air @ 23% RH & 1 ATM";
                density_imp = 0.0694;
                density_met = 1.1118;
                velocity_imp = 0.0437;
                velocity_met = 0.0651;
                specificHeat_imp = 0.243;
                specificHeat_met = 1.0174;
                energyFactor_imp = 0.894;
                energyFactor_met = 1.0058;
                break;

            case 4: // 125°F/52°C Air @ 11% RH & 1 ATM (Very hot, very dry)
                conditionName = "125°F/52°C Air @ 11% RH & 1 ATM";
                density_imp = 0.0652;
                density_met = 1.0446;
                velocity_imp = 0.0411;
                velocity_met = 0.0612;
                specificHeat_imp = 0.2445;
                specificHeat_met = 1.0237;
                energyFactor_imp = 0.842;
                energyFactor_met = 0.9473;
                break;

            default:
                // Default to STP
                conditionName = "68°F/20°C Air @ STP";
                density_imp = 0.075;
                density_met = 1.2014;
                velocity_imp = 0.0473;
                velocity_met = 0.0705;
                specificHeat_imp = 0.24;
                specificHeat_met = 1.0048;
                energyFactor_imp = 0.96;
                energyFactor_met = 1.08;
                break;
        }

        // Add properties with selected values and subscribe to changes
        var fluidDensity = new FluidProperty 
        { 
            Name = "Fluid Density", 
            ImperialValue = density_imp, 
            ImperialUnit = "lb/ft³", 
            MetricValue = density_met, 
            MetricUnit = "kg/m³",
            ConversionFactor = 16.0185
        };
        fluidDensity.PropertyChanged += OnFluidPropertyChanged;
        FluidProperties.Add(fluidDensity);
        
        var fluidViscosity = new FluidProperty 
        { 
            Name = "Fluid Viscosity", 
            ImperialValue = velocity_imp, 
            ImperialUnit = "lb/ft·h", 
            MetricValue = velocity_met, 
            MetricUnit = "kg/m·h",
            ConversionFactor = 1.4882
        };
        fluidViscosity.PropertyChanged += OnFluidPropertyChanged;
        FluidProperties.Add(fluidViscosity);
        
        var specificHeat = new FluidProperty 
        { 
            Name = "Specific Heat", 
            ImperialValue = specificHeat_imp, 
            ImperialUnit = "Btu/lb·°F", 
            MetricValue = specificHeat_met, 
            MetricUnit = "kJ/kg·°C",
            ConversionFactor = 4.1868
        };
        specificHeat.PropertyChanged += OnFluidPropertyChanged;
        FluidProperties.Add(specificHeat);
        
        var energyFactor = new FluidProperty 
        { 
            Name = "Energy Factor", 
            ImperialValue = energyFactor_imp, 
            ImperialUnit = "Btu/h·°F·ft³/min", 
            MetricValue = energyFactor_met, 
            MetricUnit = "W/°C·L/s",
            ConversionFactor = 1.1204
        };
        energyFactor.PropertyChanged += OnFluidPropertyChanged;
        FluidProperties.Add(energyFactor);

        Console.WriteLine($"MainViewModel: FluidProperties populated with {FluidProperties.Count} items");
        StatusMessage = $"{conditionName} properties loaded";
        Console.WriteLine($"MainViewModel: StatusMessage updated to '{StatusMessage}'");
        
        // Trigger recalculation with new fluid properties
        if (!_isInitializing)
        {
            PerformCalculations(-1); // -1 indicates fluid property change
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Helper method to raise PropertyChanged event with compiler support
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    protected void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Event handler for duct parameter changes
    /// </summary>
    private void OnDuctParameterChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isCalculating || _isInitializing) return;
        
        if (e.PropertyName == nameof(DuctParameter.ImperialValue) || 
            e.PropertyName == nameof(DuctParameter.MetricValue))
        {
            var parameter = sender as DuctParameter;
            if (parameter != null)
            {
                int changedRow = DuctParameters.IndexOf(parameter);
                
                // Skip recalculation if the changed parameter is locked
                if (parameter.IsLocked)
                {
                    Console.WriteLine($"Parameter changed: {parameter.Name} (row {changedRow}) - LOCKED, skipping recalculation");
                    return;
                }
                
                Console.WriteLine($"Parameter changed: {parameter.Name} (row {changedRow})");
                PerformCalculations(changedRow);
            }
        }
    }

    /// <summary>
    /// Event handler for parameter lock state changes
    /// </summary>
    private void OnParameterLockedChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DuctParameter.IsLocked))
        {
            var parameter = sender as DuctParameter;
            if (parameter != null)
            {
                int changedRow = DuctParameters.IndexOf(parameter);
                Console.WriteLine($"Parameter lock changed: {parameter.Name} (row {changedRow}) - Locked: {parameter.IsLocked}");
                
                // Validate the combination of locked parameters
                if (parameter.IsLocked)
                {
                    string validationMessage = ValidateLockedCombination();
                    if (!string.IsNullOrEmpty(validationMessage))
                    {
                        // Invalid combination - unlock the parameter and show error
                        parameter.IsLocked = false;
                        StatusMessage = validationMessage;
                        Console.WriteLine($"Validation failed: {validationMessage}");
                        return;
                    }
                }
                
                StatusMessage = parameter.IsLocked ? 
                    $"{parameter.Name} locked" : 
                    $"{parameter.Name} unlocked";
            }
        }
    }

    /// <summary>
    /// Validates the combination of locked parameters to prevent problematic sets
    /// Returns empty string if valid, error message if invalid
    /// </summary>
    private string ValidateLockedCombination()
    {
        var lockedIndices = new List<int>();
        for (int i = 0; i < 6; i++) // Only check editable parameters (0-5)
        {
            if (DuctParameters[i].IsLocked)
            {
                lockedIndices.Add(i);
            }
        }

        // If less than 3 locked, always valid
        if (lockedIndices.Count < 3)
        {
            return string.Empty;
        }

        // Check for problematic combination 1: Head loss (1), flow rate (0), and equivalent diameter (3)
        if (lockedIndices.Contains(0) && lockedIndices.Contains(1) && lockedIndices.Contains(3))
        {
            return "Error: Cannot lock Flow Rate, Head Loss, and Equivalent Diameter together (overdetermined system)";
        }

        // Check for problematic combination 2: Equivalent diameter (3), duct size X (4), and duct size Y (5)
        if (lockedIndices.Contains(3) && lockedIndices.Contains(4) && lockedIndices.Contains(5))
        {
            return "Error: Cannot lock Equivalent Diameter, Duct Size X, and Duct Size Y together (inconsistent geometry)";
        }

        return string.Empty;
    }

    /// <summary>
    /// Event handler for fluid property changes
    /// </summary>
    private void OnFluidPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isCalculating || _isInitializing) return;
        
        if (e.PropertyName == nameof(FluidProperty.ImperialValue) || 
            e.PropertyName == nameof(FluidProperty.MetricValue))
        {
            Console.WriteLine("Fluid property changed, recalculating...");
            PerformCalculations(-1); // -1 indicates fluid property change
        }
    }

    /// <summary>
    /// Perform all calculations based on which parameter changed
    /// Respects locked parameters by skipping their recalculation
    /// </summary>
    private void PerformCalculations(int changedRow)
    {
        if (_isCalculating) return;

        bool cascadeFromHeadLoss = changedRow == -1;

        try
        {
            _isCalculating = true;

            // Get fluid properties (Imperial units)
            double fluidDensity = FluidProperties[0].ImperialValue; // lb/ft³
            double fluidViscosity = FluidProperties[1].ImperialValue; // lb/ft·h

            // Get current parameter values (Imperial units)
            double flowRate = DuctParameters[0].ImperialValue; // ft³/min
            double headLoss = DuctParameters[1].ImperialValue; // in WC/100 ft
            double velocity = DuctParameters[2].ImperialValue; // fpm
            double equivDiam = DuctParameters[3].ImperialValue; // in
            double ductSizeX = DuctParameters[4].ImperialValue; // in
            double ductSizeY = DuctParameters[5].ImperialValue; // in

            // Calculate Equivalent Diameter based on which parameter changed
            // Only update if Equivalent Diameter (row 3) is not locked
            if (!DuctParameters[3].IsLocked)
            {
                if (changedRow == 4 && ductSizeX > 0 && ductSizeY > 0)
                {
                    // Duct Size X changed - calculate Equivalent Diameter from X and Y dimensions
                    equivDiam = _calculationService.CalculateEquivalentDiameter(ductSizeX, ductSizeY);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
                else if (changedRow == 5 && ductSizeX > 0 && ductSizeY > 0)
                {
                    // Duct Size Y changed - calculate Equivalent Diameter from X and Y dimensions
                    equivDiam = _calculationService.CalculateEquivalentDiameter(ductSizeX, ductSizeY);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
                else if (changedRow == 1 && flowRate > 0 && headLoss > 0)
                {
                    // Head Loss changed - iteratively calculate Equivalent Diameter
                    equivDiam = _calculationService.CalculateEquivalentDiameterFromHeadLoss(
                        flowRate, headLoss, fluidDensity, fluidViscosity);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
                else if (changedRow == 0 && flowRate > 0 && headLoss > 0 && !(ductSizeX > 0 && ductSizeY > 0))
                {
                    // Flow Rate changed and no duct dimensions - estimate Equivalent Diameter
                    equivDiam = Math.Pow(0.109136 * (Math.Pow(flowRate, 1.9) / headLoss), 1 / 5.02);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
                else if (changedRow == 2 && ductSizeX > 0 && ductSizeY > 0)
                {
                    // Velocity changed - recalculate Equivalent Diameter from duct sizes
                    equivDiam = _calculationService.CalculateEquivalentDiameter(ductSizeX, ductSizeY);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
            }

            // Re-read Equivalent Diameter in case it was updated
            equivDiam = DuctParameters[3].ImperialValue;

            // Keep the calculated Equivalent Diameter row in sync (row 6)
            UpdateCalculatedParameter(6, equivDiam);

            // Calculate Flow Area from Equivalent Diameter
            double flowArea = _calculationService.CalculateFlowArea(equivDiam);
            UpdateCalculatedParameter(7, flowArea);

            // Calculate Velocity if it wasn't the changed parameter and it's not locked
            if (changedRow != 2 && !DuctParameters[2].IsLocked && flowRate > 0 && flowArea > 0)
            {
                velocity = _calculationService.CalculateVelocityFromFlowRate(flowRate, flowArea);
                UpdateParameter(2, velocity);
                DuctParameters[2].ImperialValue = velocity;
            }
            
            // Re-read velocity to ensure we use the current value
            velocity = DuctParameters[2].ImperialValue;

            // Calculate Flow Rate if velocity changed and Flow Rate is not locked
            if (changedRow == 2 && !DuctParameters[0].IsLocked && velocity > 0 && flowArea > 0)
            {
                flowRate = _calculationService.CalculateFlowRateFromVelocity(velocity, flowArea);
                UpdateParameter(0, flowRate);
                DuctParameters[0].ImperialValue = flowRate;
            }
            
            // Re-read flowRate to ensure we use the current value
            flowRate = DuctParameters[0].ImperialValue;

            // Calculate Fluid Velocity (row 8) - same as row 2 velocity
            UpdateCalculatedParameter(8, velocity);

            // Calculate Duct Size Y and Duct Size X based on which dimension parameter changed
            // Special case: If Flow Rate AND Velocity are both locked, calculate dimensions from Flow = Velocity × Area
            bool flowAndVelocityLocked = DuctParameters[0].IsLocked && DuctParameters[2].IsLocked;
            
            if (flowAndVelocityLocked && changedRow == 4 && !DuctParameters[5].IsLocked && ductSizeX > 0 && flowRate > 0 && velocity > 0)
            {
                // Flow Rate and Velocity are locked, X changed
                // Calculate Y from: Flow Rate = Velocity × Area, where Area = X × Y / 144
                // Therefore: Y = (Flow Rate / Velocity) × 144 / X
                double requiredArea = flowRate / velocity; // ft²
                ductSizeY = (requiredArea * 144.0) / ductSizeX; // convert ft² to in²
                UpdateParameter(5, ductSizeY);
                DuctParameters[5].ImperialValue = ductSizeY;
                
                // Recalculate De from the new dimensions
                if (!DuctParameters[3].IsLocked)
                {
                    equivDiam = _calculationService.CalculateEquivalentDiameter(ductSizeX, ductSizeY);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
            }
            else if (flowAndVelocityLocked && changedRow == 5 && !DuctParameters[4].IsLocked && ductSizeY > 0 && flowRate > 0 && velocity > 0)
            {
                // Flow Rate and Velocity are locked, Y changed
                // Calculate X from: Flow Rate = Velocity × Area, where Area = X × Y / 144
                // Therefore: X = (Flow Rate / Velocity) × 144 / Y
                double requiredArea = flowRate / velocity; // ft²
                ductSizeX = (requiredArea * 144.0) / ductSizeY; // convert ft² to in²
                UpdateParameter(4, ductSizeX);
                DuctParameters[4].ImperialValue = ductSizeX;
                
                // Recalculate De from the new dimensions
                if (!DuctParameters[3].IsLocked)
                {
                    equivDiam = _calculationService.CalculateEquivalentDiameter(ductSizeX, ductSizeY);
                    UpdateParameter(3, equivDiam);
                    DuctParameters[3].ImperialValue = equivDiam;
                }
            }
            // If Equivalent Diameter changed: keep Duct Size X same, update Duct Size Y
            else if (changedRow == 3 && !DuctParameters[5].IsLocked && ductSizeX > 0 && equivDiam > 0)
            {
                ductSizeY = _calculationService.CalculateDuctSizeYFromEquivDiameter(equivDiam, ductSizeX);
                UpdateParameter(5, ductSizeY);
                DuctParameters[5].ImperialValue = ductSizeY;
            }
            // If Duct Size X changed: update both Equivalent Diameter (already done above) and Duct Size Y
            else if (changedRow == 4 && !DuctParameters[5].IsLocked && ductSizeX > 0 && equivDiam > 0)
            {
                ductSizeY = _calculationService.CalculateDuctSizeYFromEquivDiameter(equivDiam, ductSizeX);
                UpdateParameter(5, ductSizeY);
                DuctParameters[5].ImperialValue = ductSizeY;
            }
            // If Duct Size Y changed: update Equivalent Diameter (already done above) and Duct Size X
            else if (changedRow == 5 && !DuctParameters[4].IsLocked && ductSizeY > 0 && equivDiam > 0)
            {
                ductSizeX = _calculationService.CalculateDuctSizeXFromEquivDiameter(equivDiam, ductSizeY);
                UpdateParameter(4, ductSizeX);
                DuctParameters[4].ImperialValue = ductSizeX;
            }
            // Original fallback: Calculate Duct Size Y if no explicit dimension change and not locked
            else if (changedRow != 3 && changedRow != 4 && changedRow != 5 && !DuctParameters[5].IsLocked && ductSizeX > 0 && equivDiam > 0)
            {
                ductSizeY = _calculationService.CalculateDuctSizeYFromEquivDiameter(equivDiam, ductSizeX);
                UpdateParameter(5, ductSizeY);
                DuctParameters[5].ImperialValue = ductSizeY;
            }

            // Re-read dimension parameters after updates to ensure consistency
            ductSizeX = DuctParameters[4].ImperialValue;
            ductSizeY = DuctParameters[5].ImperialValue;
            equivDiam = DuctParameters[3].ImperialValue;

            // Calculate Reynolds Number
            if (fluidViscosity > 0 && equivDiam > 0)
            {
                double reynoldsNumber = _calculationService.CalculateReynoldsNumberWithFluidProperties(
                    fluidDensity, velocity, equivDiam, fluidViscosity);
                UpdateCalculatedParameter(9, reynoldsNumber);

                // Calculate Friction Factor
                double frictionFactor = _calculationService.CalculateFrictionFactorSwameeJain(reynoldsNumber);
                UpdateCalculatedParameter(10, frictionFactor);

                // Calculate Velocity Pressure
                double velocityPressure = _calculationService.CalculateVelocityPressureWithDensity(
                    velocity, fluidDensity);
                UpdateCalculatedParameter(11, velocityPressure);

                // Calculate Head Loss if it wasn't the changed parameter and it's not locked
                if (changedRow != 1 && !DuctParameters[1].IsLocked && flowRate > 0 && equivDiam > 0 && frictionFactor > 0)
                {
                    headLoss = _calculationService.CalculateHeadLossDarcyWeisbach(
                        frictionFactor, equivDiam, velocity, fluidDensity);
                    UpdateParameter(1, headLoss);
                    DuctParameters[1].ImperialValue = headLoss;

                    // Also update row 12 (duplicate Head Loss display)
                    UpdateCalculatedParameter(12, headLoss);
                }
                else if (changedRow == 1)
                {
                    // Head Loss was changed, use its current value
                    headLoss = DuctParameters[1].ImperialValue;
                    UpdateCalculatedParameter(12, headLoss);
                }
            }

            StatusMessage = "Calculations updated";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PerformCalculations: {ex.Message}");
            StatusMessage = $"Calculation error: {ex.Message}";
        }
        finally
        {
            _isCalculating = false;
        }

        if (cascadeFromHeadLoss)
        {
            PerformCalculations(1);
        }
    }

    /// <summary>
    /// Helper method to update a parameter value without triggering recalculation
    /// Only updates if the parameter is not locked
    /// </summary>
    private void UpdateParameter(int index, double imperialValue)
    {
        if (index < 0 || index >= DuctParameters.Count) return;
        
        var param = DuctParameters[index];
        
        // Skip updating if parameter is locked
        if (param.IsLocked)
        {
            Console.WriteLine($"Skipping update of locked parameter: {param.Name}");
            return;
        }
        
        // Update both imperial and metric values
        param.SetDefaults(imperialValue, imperialValue * param.ConversionFactor);
    }

    private void UpdateCalculatedParameter(int index, double imperialValue)
    {
        if (index < 0 || index >= DuctParameters.Count) return;

        var param = DuctParameters[index];
        param.SetDefaults(imperialValue, imperialValue * param.ConversionFactor);
    }
}
