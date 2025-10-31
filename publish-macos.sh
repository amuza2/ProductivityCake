#!/bin/bash
# Publish ProductivityCake for macOS (Native AOT)

echo "🚀 Publishing ProductivityCake for macOS..."

# Intel-based macOS
echo "Building for Intel (x64)..."
dotnet publish ProductivityCake/ProductivityCake.csproj -r osx-x64 -c Release --self-contained

# Apple Silicon macOS
echo "Building for Apple Silicon (arm64)..."
dotnet publish ProductivityCake/ProductivityCake.csproj -r osx-arm64 -c Release --self-contained

echo ""
echo "✅ Build complete!"
echo "📦 Intel output: ProductivityCake/bin/Release/net9.0/osx-x64/publish/"
echo "📦 ARM64 output: ProductivityCake/bin/Release/net9.0/osx-arm64/publish/"
echo ""
echo "To create a Universal binary (optional):"
echo "  lipo -create \\"
echo "    ProductivityCake/bin/Release/net9.0/osx-x64/publish/ProductivityCake \\"
echo "    ProductivityCake/bin/Release/net9.0/osx-arm64/publish/ProductivityCake \\"
echo "    -output ProductivityCake-universal"
