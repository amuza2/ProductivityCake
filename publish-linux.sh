#!/bin/bash
# Publish ProductivityCake for Linux (Native AOT)

echo "🚀 Publishing ProductivityCake for Linux x64 (Native AOT)..."
dotnet publish ProductivityCake/ProductivityCake.csproj -r linux-x64 -c Release --self-contained

echo ""
echo "✅ Build complete!"
echo "📦 Output location: ProductivityCake/bin/Release/net9.0/linux-x64/publish/"
echo ""
echo "To run the application:"
echo "  cd ProductivityCake/bin/Release/net9.0/linux-x64/publish/"
echo "  ./ProductivityCake"
