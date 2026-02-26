namespace Ductilator.Services;

/// <summary>
/// Service class that encapsulates all HVAC calculation logic
/// This is extracted from the original Form1.cs and made reusable
/// across the application and potentially other platforms
/// </summary>
public class CalculationService
{
    // Physical constants
    private const double GRAVITY = 32.174; // ft/s² (gravitational acceleration)
    private const double STANDARD_AIR_DENSITY = 0.075; // lb/ft³ for standard air

    #region Unit Conversion Constants (from Form1.cs)
    
    // Flow Rate: ft³/min ↔ L/s
    private const double FLOWRATE_CFM_TO_LS = 0.47194745;
    
    // Head Loss: in WC/100 ft ↔ Pa/m
    private const double HEADLOSS_INWC_TO_PAM = 8.1726;
    
    // Velocity: fpm ↔ m/s
    private const double VELOCITY_FPM_TO_MS = 0.00508;
    
    // Diameter/Size: in ↔ mm
    private const double SIZE_IN_TO_MM = 25.4;
    
    // Flow Area: ft² ↔ m²
    private const double AREA_FT2_TO_M2 = 0.092903;
    
    // Velocity Pressure: in WC ↔ Pa
    private const double VP_INWC_TO_PA = 248.84;
    
    // Fluid Density: lb/ft³ ↔ kg/m³
    private const double DENSITY_LBFT3_TO_KGM3 = 16.0185;
    
    // Fluid Viscosity: lb/ft·h ↔ kg/m·h
    private const double VISCOSITY_CONVERSION = 1.4882;
    
    // Specific Heat: Btu/lb·°F ↔ kJ/kg·°C
    private const double SPECIFICHEAT_CONVERSION = 4.1868;
    
    // Energy Factor: Btu/h·°F·ft³/min ↔ W/°C·L/s
    private const double ENERGYFACTOR_CONVERSION = 1.1204;
    
    #endregion

    #region Unit Conversion Methods
    
    /// <summary>
    /// Convert Flow Rate: ft³/min to L/s
    /// </summary>
    public double ConvertFlowRateImperialToMetric(double flowRateCFM)
    {
        return flowRateCFM * FLOWRATE_CFM_TO_LS;
    }
    
    /// <summary>
    /// Convert Flow Rate: L/s to ft³/min
    /// </summary>
    public double ConvertFlowRateMetricToImperial(double flowRateLS)
    {
        return flowRateLS / FLOWRATE_CFM_TO_LS;
    }
    
    /// <summary>
    /// Convert Head Loss: in WC/100 ft to Pa/m
    /// </summary>
    public double ConvertHeadLossImperialToMetric(double headLossInWC)
    {
        return headLossInWC * HEADLOSS_INWC_TO_PAM;
    }
    
    /// <summary>
    /// Convert Head Loss: Pa/m to in WC/100 ft
    /// </summary>
    public double ConvertHeadLossMetricToImperial(double headLossPaM)
    {
        return headLossPaM / HEADLOSS_INWC_TO_PAM;
    }
    
    /// <summary>
    /// Convert Velocity: fpm to m/s
    /// </summary>
    public double ConvertVelocityImperialToMetric(double velocityFPM)
    {
        return velocityFPM * VELOCITY_FPM_TO_MS;
    }
    
    /// <summary>
    /// Convert Velocity: m/s to fpm
    /// </summary>
    public double ConvertVelocityMetricToImperial(double velocityMS)
    {
        return velocityMS / VELOCITY_FPM_TO_MS;
    }
    
    /// <summary>
    /// Convert Diameter/Size: inches to mm
    /// </summary>
    public double ConvertSizeImperialToMetric(double sizeInches)
    {
        return sizeInches * SIZE_IN_TO_MM;
    }
    
    /// <summary>
    /// Convert Diameter/Size: mm to inches
    /// </summary>
    public double ConvertSizeMetricToImperial(double sizeMM)
    {
        return sizeMM / SIZE_IN_TO_MM;
    }
    
    /// <summary>
    /// Convert Flow Area: ft² to m²
    /// </summary>
    public double ConvertAreaImperialToMetric(double areaFt2)
    {
        return areaFt2 * AREA_FT2_TO_M2;
    }
    
    /// <summary>
    /// Convert Flow Area: m² to ft²
    /// </summary>
    public double ConvertAreaMetricToImperial(double areaM2)
    {
        return areaM2 / AREA_FT2_TO_M2;
    }
    
    /// <summary>
    /// Convert Velocity Pressure: in WC to Pa
    /// </summary>
    public double ConvertVelocityPressureImperialToMetric(double vpInWC)
    {
        return vpInWC * VP_INWC_TO_PA;
    }
    
    /// <summary>
    /// Convert Velocity Pressure: Pa to in WC
    /// </summary>
    public double ConvertVelocityPressureMetricToImperial(double vpPa)
    {
        return vpPa / VP_INWC_TO_PA;
    }
    
    #endregion

    #region Duct Parameter Calculations (from Form1.cs PerformCalculations method)
    
    /// <summary>
    /// Calculate Equivalent Diameter for rectangular duct
    /// De = 1.30 * (X*Y)^0.625 / (X+Y)^0.25
    /// </summary>
    public double CalculateEquivalentDiameter(double ductSizeX, double ductSizeY)
    {
        if (ductSizeX <= 0 || ductSizeY <= 0) return 0;
        
        double equivalentDiameter = 1.30 * Math.Pow(ductSizeX * ductSizeY, 0.625) / Math.Pow(ductSizeX + ductSizeY, 0.25);
        return equivalentDiameter;
    }
    
    /// <summary>
    /// Calculate Flow Area from Equivalent Diameter
    /// Area = π * (D/2)² / 144  (convert in² to ft²)
    /// </summary>
    public double CalculateFlowArea(double equivalentDiameterInches)
    {
        if (equivalentDiameterInches <= 0) return 0;
        
        double flowArea = Math.PI * Math.Pow(equivalentDiameterInches / 2, 2) / 144;
        return flowArea;
    }
    
    /// <summary>
    /// Calculate Velocity from Flow Rate and Flow Area
    /// Velocity (fpm) = Flow Rate (CFM) / Area (ft²)
    /// </summary>
    public double CalculateVelocityFromFlowRate(double flowRateCFM, double flowAreaFt2)
    {
        if (flowAreaFt2 <= 0) return 0;
        
        double velocity = flowRateCFM / flowAreaFt2;
        return velocity;
    }
    
    /// <summary>
    /// Calculate Flow Rate from Velocity and Flow Area
    /// Flow Rate (CFM) = Velocity (fpm) × Area (ft²)
    /// </summary>
    public double CalculateFlowRateFromVelocity(double velocityFPM, double flowAreaFt2)
    {
        if (flowAreaFt2 <= 0) return 0;
        
        double flowRate = velocityFPM * flowAreaFt2;
        return flowRate;
    }
    
    /// <summary>
    /// Calculate Reynolds Number using actual fluid properties
    /// Re = ρ × V × D / μ
    /// Units: (lb/ft³) × (ft/min) × (60 min/hr) × (in / 12 in/ft) / (lb/ft·h)
    /// </summary>
    public double CalculateReynoldsNumberWithFluidProperties(double fluidDensityLbFt3, double velocityFPM, 
        double equivalentDiameterInches, double fluidViscosityLbFtH)
    {
        if (fluidViscosityLbFtH <= 0 || equivalentDiameterInches <= 0) return 0;
        
        double reynoldsNumber = fluidDensityLbFt3 * velocityFPM * 60 * (equivalentDiameterInches / 12) / fluidViscosityLbFtH;
        return reynoldsNumber;
    }
    
    /// <summary>
    /// Calculate Friction Factor using Colebrook-White approximation (Swamee-Jain equation)
    /// For laminar flow (Re < 2300): f = 64/Re
    /// For turbulent flow: Swamee-Jain equation with galvanized steel roughness
    /// </summary>
    public double CalculateFrictionFactorSwameeJain(double reynoldsNumber, double relativeRoughness = 0.0005)
    {
        if (reynoldsNumber <= 0) return 0;
        
        if (reynoldsNumber < 2300)
        {
            // Laminar flow: f = 64/Re
            return 64 / reynoldsNumber;
        }
        else
        {
            // Turbulent flow: Swamee-Jain approximation
            // f = 0.25 / [log10(ε/(3.7D) + 5.74/Re^0.9)]²
            double logTerm = Math.Log10(relativeRoughness / 3.7 + 5.74 / Math.Pow(reynoldsNumber, 0.9));
            double frictionFactor = 0.25 / Math.Pow(logTerm, 2);
            return frictionFactor;
        }
    }
    
    /// <summary>
    /// Calculate Velocity Pressure using actual fluid density
    /// VP = (ρ/ρ_std) × (V/4005)²
    /// The 4005 constant converts to in WC for standard air (0.075 lb/ft³)
    /// </summary>
    public double CalculateVelocityPressureWithDensity(double velocityFPM, double fluidDensityLbFt3)
    {
        if (velocityFPM <= 0) return 0;
        
        double densityRatio = fluidDensityLbFt3 / STANDARD_AIR_DENSITY;
        double velocityPressure = densityRatio * Math.Pow(velocityFPM / 4005, 2);
        return velocityPressure;
    }
    
    /// <summary>
    /// Calculate Head Loss using Darcy-Weisbach equation with actual fluid properties
    /// ΔP/100ft = f × (100 ft / D) × (ρ × V²) / (2 × 1097)
    /// The constant 1097 converts (lb/ft³)(ft/min)² to in WC
    /// </summary>
    public double CalculateHeadLossDarcyWeisbach(double frictionFactor, double equivalentDiameterInches, 
        double velocityFPM, double fluidDensityLbFt3)
    {
        if (equivalentDiameterInches <= 0 || velocityFPM <= 0) return 0;
        
        double length = 100; // per 100 ft
        double diameterFt = equivalentDiameterInches / 12; // convert inches to feet
        
        // ΔP = f × (L/D) × (ρ × V²) / (2 × 1097)
        double headLoss = frictionFactor * (length / diameterFt) * (fluidDensityLbFt3 * Math.Pow(velocityFPM, 2)) / (2 * 1097);
        return headLoss;
    }
    
    /// <summary>
    /// Calculate Equivalent Diameter from Head Loss (iterative approach)
    /// Uses Newton-Raphson iteration to solve the implicit Darcy-Weisbach equation
    /// </summary>
    public double CalculateEquivalentDiameterFromHeadLoss(double flowRateCFM, double headLossInWC, 
        double fluidDensityLbFt3, double fluidViscosityLbFtH, int maxIterations = 5)
    {
        if (flowRateCFM <= 0 || headLossInWC <= 0) return 0;
        
        // Initial estimate using simplified formula for standard air
        double equivalentDiameter = Math.Pow(0.109136 * (Math.Pow(flowRateCFM, 1.9) / headLossInWC), 1 / 5.02);
        
        // Refine using actual fluid properties (Newton-Raphson iteration)
        for (int iter = 0; iter < maxIterations; iter++)
        {
            double D_ft = equivalentDiameter / 12;
            double A = Math.PI * Math.Pow(D_ft / 2, 2);
            double V = flowRateCFM / (A * 60); // ft/s
            double Re = fluidDensityLbFt3 * V * 60 * (equivalentDiameter / 12) / fluidViscosityLbFtH;
            
            double f;
            if (Re < 2300)
            {
                f = 64 / Re;
            }
            else
            {
                double relRough = 0.0005;
                double logTerm = Math.Log10(relRough / 3.7 + 5.74 / Math.Pow(Re, 0.9));
                f = 0.25 / Math.Pow(logTerm, 2);
            }
            
            // Calculate pressure loss with current diameter
            double calcPL = f * (100 / D_ft) * (fluidDensityLbFt3 * Math.Pow(V * 60, 2)) / (2 * 1097);
            
            // Adjust diameter based on error
            double error = (calcPL - headLossInWC) / headLossInWC;
            if (Math.Abs(error) < 0.001) break; // Converged
            
            // Adjust: if calcPL > target, need larger D
            equivalentDiameter *= Math.Pow(calcPL / headLossInWC, 0.2);
        }
        
        return equivalentDiameter;
    }
    
    /// <summary>
    /// Calculate Duct Size Y from Equivalent Diameter and Duct Size X
    /// Using inverse of equivalent diameter formula (approximation for area)
    /// Y = (π × (De/2)²) / X
    /// </summary>
    public double CalculateDuctSizeYFromEquivDiameter(double equivalentDiameterInches, double ductSizeX)
    {
        if (equivalentDiameterInches <= 0 || ductSizeX <= 0) return 0;
        
        double ductSizeY = (Math.PI * Math.Pow(equivalentDiameterInches / 2, 2)) / ductSizeX;
        return ductSizeY;
    }
    
    /// <summary>
    /// Calculate Duct Size X from Equivalent Diameter and Duct Size Y
    /// Using inverse of equivalent diameter formula (approximation for area)
    /// X = (π × (De/2)²) / Y
    /// </summary>
    public double CalculateDuctSizeXFromEquivDiameter(double equivalentDiameterInches, double ductSizeY)
    {
        if (equivalentDiameterInches <= 0 || ductSizeY <= 0) return 0;
        
        double ductSizeX = (Math.PI * Math.Pow(equivalentDiameterInches / 2, 2)) / ductSizeY;
        return ductSizeX;
    }
    
    #endregion

    #region Legacy Methods (kept for backward compatibility)

    /// <summary>
    /// Calculate velocity from flow rate and duct diameter
    /// Velocity = Flow Rate / Cross-sectional Area
    /// </summary>
    public double CalculateVelocity(double flowRateCFM, double diameterInches)
    {
        if (diameterInches <= 0) return 0;
        
        // Convert diameter to feet
        double diameterFeet = diameterInches / 12.0;
        
        // Calculate cross-sectional area (circular duct)
        double radiusFeet = diameterFeet / 2.0;
        double areaSquareFeet = System.Math.PI * radiusFeet * radiusFeet;
        
        // Velocity in ft/min = CFM / area, convert to ft/s
        double velocityFtPerMin = flowRateCFM / areaSquareFeet;
        double velocityFtPerSec = velocityFtPerMin / 60.0;
        
        return velocityFtPerSec;
    }

    /// <summary>
    /// Calculate Reynolds number
    /// Re = (ρ × V × D) / μ
    /// where: ρ = density, V = velocity, D = diameter, μ = dynamic viscosity
    /// </summary>
    public double CalculateReynoldsNumber(double densityLbPerFt3, double velocityFtPerSec, 
        double diameterFeet, double dynamicViscosityLbPerFtSec)
    {
        if (velocityFtPerSec <= 0) return 0;
        
        double reynolds = (densityLbPerFt3 * velocityFtPerSec * diameterFeet) / dynamicViscosityLbPerFtSec;
        return reynolds;
    }

    /// <summary>
    /// Calculate friction factor using the Haaland equation (approximation of Colebrook-White)
    /// Applicable for both laminar and turbulent flow
    /// </summary>
    public double CalculateFrictionFactor(double reynoldsNumber, double relativeRoughness = 0.000005)
    {
        if (reynoldsNumber <= 0) return 0;

        // Laminar flow (Re < 2300)
        if (reynoldsNumber < 2300)
        {
            return 64.0 / reynoldsNumber;
        }

        // Turbulent flow (Re >= 2300) - Haaland equation
        // 1/√f = -1.8 × log₁₀((ε/D)/3.7 + 6.9/Re)
        double term1 = relativeRoughness / 3.7;
        double term2 = 6.9 / reynoldsNumber;
        double logTerm = System.Math.Log10(term1 + term2);
        double sqrtFInverse = -1.8 * logTerm;
        
        if (sqrtFInverse <= 0) return 0.01; // Safety check
        
        double frictionFactor = 1.0 / (sqrtFInverse * sqrtFInverse);
        return frictionFactor;
    }

    /// <summary>
    /// Calculate pressure drop using the Darcy-Weisbach equation
    /// ΔP = f × (L/D) × (ρ × V²) / (2 × g)
    /// where: f = friction factor, L = duct length, D = diameter, ρ = density, V = velocity, g = gravity
    /// Result in inches of water gauge (in. wg)
    /// </summary>
    public double CalculatePressureDrop(double frictionFactor, double lengthFeet, double diameterFeet,
        double densityLbPerFt3, double velocityFtPerSec)
    {
        if (diameterFeet <= 0 || velocityFtPerSec <= 0) return 0;

        // Darcy-Weisbach equation (feet of fluid)
        double pressureDropFeetOfFluid = frictionFactor 
            * (lengthFeet / diameterFeet) 
            * (densityLbPerFt3 * velocityFtPerSec * velocityFtPerSec) 
            / (2.0 * GRAVITY);

        // Convert feet of fluid to inches of water gauge
        // 1 foot of fluid = 12 inches of fluid
        double pressureDropInWg = pressureDropFeetOfFluid * 12.0;

        return pressureDropInWg;
    }

    /// <summary>
    /// Calculate velocity pressure
    /// VP = (ρ × V²) / (2 × g)
    /// Result in inches of water gauge
    /// </summary>
    public double CalculateVelocityPressure(double densityLbPerFt3, double velocityFtPerSec)
    {
        if (velocityFtPerSec <= 0) return 0;

        // In feet of fluid
        double velocityPressureFeetOfFluid = (densityLbPerFt3 * velocityFtPerSec * velocityFtPerSec) 
            / (2.0 * GRAVITY);

        // Convert to inches of water gauge
        double velocityPressureInWg = velocityPressureFeetOfFluid * 12.0;

        return velocityPressureInWg;
    }

    /// <summary>
    /// Convert Imperial units to Metric units (and vice versa)
    /// </summary>
    public double ConvertImperialToMetric(double imperialValue, string unitType)
    {
        return unitType switch
        {
            "density" => imperialValue * 16.0185, // lb/ft³ to kg/m³
            "viscosity" => imperialValue * 1.488, // lb/(ft·s) to Pa·s
            "length" => imperialValue * 0.3048, // feet to meters
            "velocity" => imperialValue * 0.3048, // ft/s to m/s
            "flowrate" => imperialValue * 0.4719, // CFM to L/s
            "pressure" => imperialValue * 248.84, // in. wg to Pa
            _ => imperialValue
        };
    }

    /// <summary>
    /// Convert Metric units to Imperial units
    /// </summary>
    public double ConvertMetricToImperial(double metricValue, string unitType)
    {
        return unitType switch
        {
            "density" => metricValue / 16.0185, // kg/m³ to lb/ft³
            "viscosity" => metricValue / 1.488, // Pa·s to lb/(ft·s)
            "length" => metricValue / 0.3048, // meters to feet
            "velocity" => metricValue / 0.3048, // m/s to ft/s
            "flowrate" => metricValue / 0.4719, // L/s to CFM
            "pressure" => metricValue / 248.84, // Pa to in. wg
            _ => metricValue
        };
    }
    
    #endregion
}
