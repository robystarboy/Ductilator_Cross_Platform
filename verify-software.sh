#!/bin/bash

# SOFTWARE VERIFICATION SCRIPT
# Run this to check your development environment
# Usage: bash software-verification.sh

echo "==============================================="
echo "DUCTILATOR MIGRATION - SOFTWARE VERIFICATION"
echo "Date: February 24, 2026"
echo "==============================================="
echo ""

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Counter for issues found
ISSUES=0

# Check 1: .NET SDK
echo -n "Checking .NET SDK ... "
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ FOUND${NC}"
    echo "  Version: $DOTNET_VERSION"
    
    # Check if it's version 6.0 or higher
    MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d. -f1)
    if [ "$MAJOR_VERSION" -lt 6 ]; then
        echo -e "${RED}  ✗ WARNING: .NET 6.0+ required (found $DOTNET_VERSION)${NC}"
        ((ISSUES++))
    fi
else
    echo -e "${RED}✗ NOT FOUND${NC}"
    echo -e "  ${YELLOW}ACTION: Install .NET 6.0 SDK${NC}"
    echo "  Download: https://dotnet.microsoft.com/download/dotnet/6.0"
    ((ISSUES++))
fi
echo ""

# Check 2: .NET SDKs Available
echo -n "Checking available .NET SDKs ... "
if command -v dotnet &> /dev/null; then
    echo -e "${GREEN}✓ CHECKING${NC}"
    dotnet --list-sdks | sed 's/^/  /'
else
    echo -e "${RED}SKIPPED (dotnet not found)${NC}"
fi
echo ""

# Check 3: Git
echo -n "Checking Git ... "
if command -v git &> /dev/null; then
    GIT_VERSION=$(git --version)
    echo -e "${GREEN}✓ FOUND${NC}"
    echo "  $GIT_VERSION"
else
    echo -e "${YELLOW}⊘ NOT FOUND (Optional)${NC}"
    echo "  Install with: brew install git"
fi
echo ""

# Check 4: Java (for some build tools)
echo -n "Checking Java ... "
if command -v java &> /dev/null; then
    JAVA_VERSION=$(java -version 2>&1 | head -1)
    echo -e "${GREEN}✓ FOUND${NC}"
    echo "  $JAVA_VERSION"
else
    echo -e "${YELLOW}⊘ NOT FOUND (Optional)${NC}"
fi
echo ""

# Check 5: Node.js (optional)
echo -n "Checking Node.js ... "
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓ FOUND${NC}"
    echo "  $NODE_VERSION"
else
    echo -e "${YELLOW}⊘ NOT FOUND (Optional)${NC}"
fi
echo ""

# Check 6: VS Code (optional)
echo -n "Checking Visual Studio Code ... "
if command -v code &> /dev/null; then
    echo -e "${GREEN}✓ FOUND${NC}"
else
    echo -e "${YELLOW}⊘ NOT FOUND (Optional)${NC}"
fi
echo ""

# Check 7: curl (usually present)
echo -n "Checking curl ... "
if command -v curl &> /dev/null; then
    echo -e "${GREEN}✓ FOUND${NC}"
else
    echo -e "${RED}✗ NOT FOUND (Required for downloads)${NC}"
    ((ISSUES++))
fi
echo ""

# Check 8: Disk Space
echo -n "Checking disk space ... "
AVAILABLE_SPACE=$(df / | awk 'NR==2 {print $4}')
if [ "$AVAILABLE_SPACE" -gt 500000 ]; then
    echo -e "${GREEN}✓ OK${NC}"
    echo "  Available: $(df -h / | awk 'NR==2 {print $4}')B"
else
    echo -e "${YELLOW}⚠ LOW${NC}"
    echo "  Available: $(df -h / | awk 'NR==2 {print $4}')B (needs 500MB+)"
fi
echo ""

# Check 9: macOS specific
if [[ "$OSTYPE" == "darwin"* ]]; then
    echo "=== macOS Specific Checks ==="
    
    # Check Xcode CLI tools
    echo -n "Checking Xcode Command Line Tools ... "
    if xcode-select -p &> /dev/null; then
        echo -e "${GREEN}✓ FOUND${NC}"
        echo "  Path: $(xcode-select -p)"
    else
        echo -e "${YELLOW}⊘ NOT FOUND (Optional but recommended)${NC}"
        echo "  Install with: xcode-select --install"
    fi
    echo ""
    
    # Check Homebrew
    echo -n "Checking Homebrew ... "
    if command -v brew &> /dev/null; then
        BREW_VERSION=$(brew --version | head -1)
        echo -e "${GREEN}✓ FOUND${NC}"
        echo "  $BREW_VERSION"
    else
        echo -e "${YELLOW}⊘ NOT FOUND (Optional but recommended)${NC}"
    fi
    echo ""
fi

# Check 10: NuGet access
echo -n "Checking NuGet.org access ... "
if curl -s --connect-timeout 5 https://api.nuget.org/v3/index.json > /dev/null; then
    echo -e "${GREEN}✓ ACCESSIBLE${NC}"
else
    echo -e "${YELLOW}⚠ UNABLE TO REACH (may be firewall/network issue)${NC}"
    ((ISSUES++))
fi
echo ""

# Summary
echo "==============================================="
echo "VERIFICATION SUMMARY"
echo "==============================================="

if [ $ISSUES -eq 0 ]; then
    echo -e "${GREEN}✓ ALL CRITICAL CHECKS PASSED${NC}"
    echo ""
    echo "Your environment is ready for migration!"
    echo "Proceed with Phase 1: Update Ductilator.csproj"
else
    echo -e "${RED}✗ $ISSUES ISSUE(S) FOUND${NC}"
    echo ""
    echo "Action required:"
    echo "1. Install missing .NET 6.0 SDK"
    echo "2. Verify internet connection"
    echo "3. See instructions above"
fi
echo ""

# System Info
echo "System Information:"
uname -a | sed 's/^/  /'
echo ""
