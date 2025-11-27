#!/bin/bash

# ProductivityCake AppImage Build Script
# This script builds a universal AppImage for all Linux distributions

set -e  # Exit on error

VERSION="1.2.0"
ARCH="x86_64"
APP_NAME="ProductivityCake"
APPDIR="${APP_NAME}.AppDir"

echo "🎂 Building ${APP_NAME} AppImage v${VERSION}..."
echo ""

# Check for required tools
if ! command -v appimagetool &> /dev/null; then
    echo "⚠️  appimagetool not found. Downloading..."
    wget -q https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-${ARCH}.AppImage -O appimagetool
    chmod +x appimagetool
    APPIMAGETOOL="./appimagetool"
else
    APPIMAGETOOL="appimagetool"
fi

# Clean previous builds
echo "🧹 Cleaning previous builds..."
rm -rf ./publish/linux-x64
rm -rf ./${APPDIR}/usr

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

# Create AppDir structure
echo "📁 Creating AppImage directory structure..."
mkdir -p ${APPDIR}/usr/bin
mkdir -p ${APPDIR}/usr/lib
mkdir -p ${APPDIR}/usr/share/applications
mkdir -p ${APPDIR}/usr/share/icons/hicolor/256x256/apps

# Copy application files
echo "📋 Copying application files..."
cp ./publish/linux-x64/ProductivityCake ${APPDIR}/usr/bin/

# Copy shared libraries
if [ -f "./publish/linux-x64/libSkiaSharp.so" ]; then
    cp ./publish/linux-x64/libSkiaSharp.so ${APPDIR}/usr/lib/
    echo "✅ Copied libSkiaSharp.so"
fi
if [ -f "./publish/linux-x64/libHarfBuzzSharp.so" ]; then
    cp ./publish/linux-x64/libHarfBuzzSharp.so ${APPDIR}/usr/lib/
    echo "✅ Copied libHarfBuzzSharp.so"
fi

# Copy any other .so files
for lib in ./publish/linux-x64/*.so; do
    if [ -f "$lib" ]; then
        cp "$lib" ${APPDIR}/usr/lib/
        echo "✅ Copied $(basename $lib)"
    fi
done

# Copy alarm.mp3 (check multiple locations)
if [ -f "./publish/linux-x64/alarm.mp3" ]; then
    cp ./publish/linux-x64/alarm.mp3 ${APPDIR}/usr/bin/
    echo "✅ Copied alarm.mp3 from publish directory"
elif [ -f "./publish/linux-x64/Assets/alarm.mp3" ]; then
    cp ./publish/linux-x64/Assets/alarm.mp3 ${APPDIR}/usr/bin/
    echo "✅ Copied alarm.mp3 from Assets directory"
elif [ -f "./ProductivityCake/Assets/alarm.mp3" ]; then
    cp ./ProductivityCake/Assets/alarm.mp3 ${APPDIR}/usr/bin/
    echo "✅ Copied alarm.mp3 from source Assets directory"
else
    echo "⚠️  Warning: alarm.mp3 not found, sound notifications may not work"
fi

# Copy icon (convert from PNG to use as app icon)
if [ -f "ProductivityCake/Assets/icons8-cake-96.png" ]; then
    cp ProductivityCake/Assets/icons8-cake-96.png ${APPDIR}/productivitycake.png
    cp ProductivityCake/Assets/icons8-cake-96.png ${APPDIR}/usr/share/icons/hicolor/256x256/apps/productivitycake.png
else
    echo "⚠️  Warning: Icon file not found"
fi

# Copy desktop file
cp ${APPDIR}/ProductivityCake.desktop ${APPDIR}/usr/share/applications/

# Make AppRun executable
chmod +x ${APPDIR}/AppRun

# Build AppImage
echo ""
echo "🔨 Building AppImage..."
ARCH=${ARCH} ${APPIMAGETOOL} ${APPDIR} ${APP_NAME}-${VERSION}-${ARCH}.AppImage

echo ""
echo "✅ AppImage created successfully!"
echo ""
echo "📦 Output: ./${APP_NAME}-${VERSION}-${ARCH}.AppImage"
echo ""
echo "🚀 To run the AppImage:"
echo "   chmod +x ${APP_NAME}-${VERSION}-${ARCH}.AppImage"
echo "   ./${APP_NAME}-${VERSION}-${ARCH}.AppImage"
echo ""
echo "📤 To distribute:"
echo "   Upload ${APP_NAME}-${VERSION}-${ARCH}.AppImage to GitHub Releases"
echo ""
