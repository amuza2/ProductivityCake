#!/bin/bash
# Publish ProductivityCake for Windows (Native AOT)

echo "🚀 Publishing ProductivityCake for Windows x64 (Native AOT)..."
dotnet publish ProductivityCake/ProductivityCake.csproj -r win-x64 -c Release --self-contained

echo ""
echo "✅ Build complete!"
echo "📦 Output location: ProductivityCake/bin/Release/net9.0/win-x64/publish/"
echo ""
echo "To run the application on Windows:"
echo "  Navigate to: ProductivityCake/bin/Release/net9.0/win-x64/publish/"
echo "  Run: ProductivityCake.exe"
