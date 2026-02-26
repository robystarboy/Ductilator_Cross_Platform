#!/bin/bash

# Quick test script for ComboBox dropdown fix
# Builds and runs the application

echo "═══════════════════════════════════════════════════════"
echo "Testing Standard Properties ComboBox Fix"
echo "═══════════════════════════════════════════════════════"
echo ""

cd "/Users/robystar/Library/CloudStorage/GoogleDrive-robystar@gmail.com/My Drive/PROJECTS/DEVELPOMENT/VISUAL STUDIO/Ductilator_Cross-Platform"

echo "🔨 Building project..."
dotnet build --no-restore

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ Build successful"
echo ""
echo "🚀 Launching application..."
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "TEST CHECKLIST:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "1. ✓ Click the 'Standard Properties' dropdown"
echo "2. ✓ Verify you can see all 5 options"
echo "3. ✓ Select '75°F/25°C Air @ 50% RH & 1 ATM'"
echo "4. ✓ Verify fluid properties update"
echo "5. ✓ Verify status bar shows selected condition"
echo "6. ✓ Try selecting different options"
echo "7. ✓ Verify dropdown closes after selection"
echo "8. ✓ Verify selection highlights correctly"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

dotnet run --no-build
