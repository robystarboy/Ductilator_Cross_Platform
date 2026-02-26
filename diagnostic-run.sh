#!/bin/bash

echo "╔═══════════════════════════════════════════════════════════════╗"
echo "║  DIAGNOSTIC BUILD & RUN - Window Not Appearing Issue         ║"
echo "╚═══════════════════════════════════════════════════════════════╝"
echo ""

cd "/Users/robystar/Library/CloudStorage/GoogleDrive-robystar@gmail.com/My Drive/PROJECTS/DEVELPOMENT/VISUAL STUDIO/Ductilator_Cross-Platform"

echo "🧹 Cleaning previous build artifacts..."
dotnet clean > /dev/null 2>&1

echo "📦 Restoring packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ Package restore failed!"
    exit 1
fi

echo "✅ Packages restored"
echo ""

echo "🔨 Building project with detailed output..."
dotnet build --no-restore

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ BUILD FAILED!"
    echo ""
    echo "Please check the build errors above."
    exit 1
fi

echo ""
echo "✅ Build successful!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "  LAUNCHING APPLICATION WITH DIAGNOSTIC LOGGING"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Watch for console output below:"
echo "- MainWindow initialization messages"
echo "- MainViewModel initialization messages"
echo "- Any error messages or exceptions"
echo ""
echo "If the window doesn't appear, press Ctrl+C and report the output."
echo ""
echo "Starting in 2 seconds..."
sleep 2
echo ""

dotnet run --no-build
