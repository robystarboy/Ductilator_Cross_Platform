#!/bin/bash

echo "╔══════════════════════════════════════════════════════════════╗"
echo "║  ComboBox Data Binding Fix - Final Verification             ║"
echo "╚══════════════════════════════════════════════════════════════╝"
echo ""

cd "/Users/robystar/Library/CloudStorage/GoogleDrive-robystar@gmail.com/My Drive/PROJECTS/DEVELPOMENT/VISUAL STUDIO/Ductilator_Cross-Platform"

echo "🔍 Verifying implementation..."
echo ""

# Check for data binding in XAML
if grep -q 'SelectedIndex="{Binding SelectedConditionIndex}"' MainWindow.axaml; then
    echo "✅ XAML: Data binding found"
else
    echo "❌ XAML: Data binding NOT found"
    exit 1
fi

# Check for property in ViewModel
if grep -q "public int SelectedConditionIndex" ViewModels/MainViewModel.cs; then
    echo "✅ ViewModel: SelectedConditionIndex property found"
else
    echo "❌ ViewModel: Property NOT found"
    exit 1
fi

# Check that event handler is removed
if grep -q "SelectionChanged=" MainWindow.axaml; then
    echo "⚠️  WARNING: SelectionChanged event still in XAML"
    exit 1
else
    echo "✅ XAML: No SelectionChanged event (correct!)"
fi

# Check that event handler is removed from code-behind
if grep -q "OnStandardPropertiesChanged" MainWindow.axaml.cs; then
    echo "⚠️  WARNING: Event handler still in code-behind"
    exit 1
else
    echo "✅ Code-behind: No event handler (correct!)"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "  All checks passed! Proper MVVM data binding implemented."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "🔨 Building project..."
dotnet build --no-restore -v quiet

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "✅ Build successful!"
echo ""
echo "🚀 Launching application..."
echo ""
echo "┌────────────────────────────────────────────────────────────┐"
echo "│ TEST THE DROPDOWN:                                         │"
echo "├────────────────────────────────────────────────────────────┤"
echo "│ 1. Click 'Standard Properties' dropdown                    │"
echo "│ 2. Select '75°F/25°C Air @ 50% RH & 1 ATM'                │"
echo "│ 3. Verify fluid properties update                          │"
echo "│ 4. Check status bar message                                │"
echo "│ 5. Try selecting other options                             │"
echo "│                                                            │"
echo "│ Expected: Dropdown works smoothly, properties update!      │"
echo "└────────────────────────────────────────────────────────────┘"
echo ""

dotnet run --no-build
