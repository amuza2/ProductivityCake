#!/bin/bash

# ProductivityCake Linux Build Script
# This script builds a native AOT binary for Linux x64

set -e  # Exit on error

echo "🎂 Building ProductivityCake for Linux..."
echo ""

# Clean previous builds
if [ -d "./publish/linux-x64" ]; then
    echo "🧹 Cleaning previous build..."
    rm -rf ./publish/linux-x64
fi

# Publish the application
echo "📦 Publishing native AOT binary..."
dotnet publish ProductivityCake/ProductivityCake.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained \
  -o ./publish/linux-x64

echo ""
echo "✅ Build completed successfully!"
echo ""
echo "📍 Output location: ./publish/linux-x64/ProductivityCake"
echo ""

# Check if alarm.mp3 was copied
if [ ! -f "./publish/linux-x64/alarm.mp3" ]; then
    echo "⚠️  Warning: alarm.mp3 not found in output directory"
    echo "   Checking Assets folder..."
    if [ -f "./publish/linux-x64/Assets/alarm.mp3" ]; then
        echo "   Found in Assets folder, copying to root..."
        cp ./publish/linux-x64/Assets/alarm.mp3 ./publish/linux-x64/
    fi
fi

# Create distributable archive
echo "📦 Creating distributable archive..."
cd publish/linux-x64
if [ -f "alarm.mp3" ]; then
    tar -czf ProductivityCake-linux-x64-v1.1.0.tar.gz ProductivityCake alarm.mp3
else
    echo "⚠️  Creating archive without alarm.mp3 (file not found)"
    tar -czf ProductivityCake-linux-x64-v1.1.0.tar.gz ProductivityCake
fi
cd ../..

echo ""
echo "✅ Archive created: ./publish/linux-x64/ProductivityCake-linux-x64-v1.1.0.tar.gz"
echo ""
echo "🚀 To run the application:"
echo "   cd publish/linux-x64"
echo "   ./ProductivityCake"
echo ""
