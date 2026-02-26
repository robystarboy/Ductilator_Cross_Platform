using Avalonia;

namespace Ductilator
{
    /// <summary>
    /// Global variables for calculation state
    /// </summary>
    static class GlobalVar
    {
        public static double FluidDensity;
        public static double FluidViscosity;
        public static double SpecificHeat;
        public static double EnergyFactor;
        public static double FlowRate;
        public static double HeadLoss;
        public static double Velocity;
        public static double EquivalentDiameter;
        public static double DuctSizeX;
        public static double DuctSizeY;
        public static double FlowArea;
        public static double FluidVelocity;
        public static double ReynoldsNumber;
        public static double FrictionFactor;
        public static double VelocityPressure;
        public static double FrictionFactorA;
        public static double FrictionFactorB;
        public static string Set_Units;
    }

    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppBuilder is called.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, using the fluent API to set up the App
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}