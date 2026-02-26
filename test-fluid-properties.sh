#!/bin/bash

# Quick Build and Run Test Script
# Tests the Fluid Properties enhancements

echo "================================================"
echo "Ductilator - Fluid Properties Enhancement Test"
echo "================================================"
echo ""

# Navigate to project directory
cd "/Users/robystar/Library/CloudStorage/GoogleDrive-robystar@gmail.com/My Drive/PROJECTS/DEVELPOMENT/VISUAL STUDIO/Ductilator_Cross-Platform"

# Restore packages
echo "📦 Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ Package restore failed!"
    exit 1
fi

echo "✅ Packages restored successfully"
echo ""

# Build the project
echo "🔨 Building project..."
dotnet build --no-restore

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ Build successful"
echo ""

# Run the application
echo "🚀 Launching application..."
echo ""
echo "Test Checklist:"
echo "1. ✓ Click 'Load Air Properties' - verify 6 significant digits"
echo "2. ✓ Edit Imperial Density to 100 - verify Metric updates to ~1601.85"
echo "3. ✓ Edit Metric Density to 1000 - verify Imperial updates to ~62.428"
echo "4. ✓ Try typing letters in a field - verify they're blocked"
echo "5. ✓ Try typing decimals (0.123) - verify they work"
echo "6. ✓ Try scientific notation (1.23e-5) - verify it works"
echo ""

dotnet run --no-build
