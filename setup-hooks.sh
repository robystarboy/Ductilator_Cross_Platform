#!/bin/bash
# Setup script to install Git hooks for automatic versioning
# Run this after cloning the repository

HOOKS_DIR=".git/hooks"
REPO_HOOKS_DIR="hooks"

echo "🔧 Setting up Git hooks..."

# Check if hooks directory exists in repo
if [ ! -d "$REPO_HOOKS_DIR" ]; then
    echo "⚠ Hooks directory not found. Creating hooks directory..."
    mkdir -p "$REPO_HOOKS_DIR"
fi

# Copy hooks to .git/hooks
if [ -f "$REPO_HOOKS_DIR/pre-commit" ]; then
    cp "$REPO_HOOKS_DIR/pre-commit" "$HOOKS_DIR/pre-commit"
    chmod +x "$HOOKS_DIR/pre-commit"
    echo "✓ pre-commit hook installed"
fi

if [ -f "$REPO_HOOKS_DIR/post-commit" ]; then
    cp "$REPO_HOOKS_DIR/post-commit" "$HOOKS_DIR/post-commit"
    chmod +x "$HOOKS_DIR/post-commit"
    echo "✓ post-commit hook installed"
fi

echo "✓ Git hooks setup complete!"
echo ""
echo "📝 Versioning Protocol:"
echo "  - Version format: YYYY.MM.DD.BuildNumber"
echo "  - Version automatically increments on commit"
echo "  - Same day commits increment build number"
echo "  - New day resets build number to 001"
echo ""
echo "🚀 Your next commit will automatically:"
echo "  1. Update version in .csproj and MainViewModel.cs"
echo "  2. Update the version label in the status bar"
echo "  3. Push changes to GitHub"

