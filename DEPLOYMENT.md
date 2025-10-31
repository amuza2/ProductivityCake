# ProductivityCake - Deployment Guide

This guide explains how to build and deploy ProductivityCake with Native AOT compilation.

## 🚀 Native AOT Benefits

- ⚡ **Faster Startup**: Near-instant application launch
- 💾 **Smaller Memory Footprint**: Reduced RAM usage
- 📦 **Self-Contained**: No .NET runtime required
- 🔒 **Better Security**: No JIT compilation
- 📉 **Smaller Size**: Optimized binary with trimming

## 📋 Prerequisites

- .NET 9.0 SDK or later
- Platform-specific build tools:
  - **Windows**: Visual Studio 2022 or Build Tools
  - **Linux**: `clang` and `zlib1g-dev`
  - **macOS**: Xcode Command Line Tools

## 🛠️ Building for Different Platforms

### Linux (x64)

```bash
# Using the provided script
./publish-linux.sh

# Or manually
dotnet publish ProductivityCake/ProductivityCake.csproj -r linux-x64 -c Release --self-contained
```

**Output**: `ProductivityCake/bin/Release/net9.0/linux-x64/publish/`

### Windows (x64)

```bash
# Using the provided script
./publish-windows.sh

# Or manually
dotnet publish ProductivityCake/ProductivityCake.csproj -r win-x64 -c Release --self-contained
```

**Output**: `ProductivityCake/bin/Release/net9.0/win-x64/publish/`

### macOS (Intel)

```bash
dotnet publish ProductivityCake/ProductivityCake.csproj -r osx-x64 -c Release --self-contained
```

**Output**: `ProductivityCake/bin/Release/net9.0/osx-x64/publish/`

### macOS (Apple Silicon)

```bash
dotnet publish ProductivityCake/ProductivityCake.csproj -r osx-arm64 -c Release --self-contained
```

**Output**: `ProductivityCake/bin/Release/net9.0/osx-arm64/publish/`

### macOS Universal Binary (Both Intel & Apple Silicon)

```bash
# Build both architectures first
./publish-macos.sh

# Then create universal binary
lipo -create \
  ProductivityCake/bin/Release/net9.0/osx-x64/publish/ProductivityCake \
  ProductivityCake/bin/Release/net9.0/osx-arm64/publish/ProductivityCake \
  -output ProductivityCake-universal
```

## 📦 Distribution

After building, the `publish` folder contains everything needed to run the application:

```
publish/
├── ProductivityCake (or ProductivityCake.exe on Windows)
├── Assets/
│   └── alarm.mp3
└── [Native dependencies]
```

### Creating Distribution Packages

#### Linux (AppImage/Flatpak)
Package the `publish` folder into an AppImage or Flatpak for easy distribution.

#### Windows (Installer)
Use tools like:
- **Inno Setup**: Create a Windows installer
- **WiX Toolset**: Create MSI packages
- **NSIS**: Lightweight installer

#### macOS (App Bundle)
Create a `.app` bundle:
```bash
mkdir -p ProductivityCake.app/Contents/MacOS
cp -r publish/* ProductivityCake.app/Contents/MacOS/
# Add Info.plist and icon
```

## 🔧 Configuration Details

### Project Settings (ProductivityCake.csproj)

```xml
<PropertyGroup>
    <!-- Enable Native AOT -->
    <PublishAot>true</PublishAot>
    
    <!-- Keep globalization support -->
    <InvariantGlobalization>false</InvariantGlobalization>
    
    <!-- Strip debug symbols for smaller size -->
    <StripSymbols>true</StripSymbols>
    
    <!-- Already enabled - ensures XAML is compiled -->
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
</PropertyGroup>
```

### Trimmer Configuration (TrimmerRoots.xml)

Preserves ViewModels, Models, Services, and Converters from being trimmed by the AOT compiler.

## 📊 Expected Binary Sizes

Approximate sizes after Native AOT compilation:

- **Linux x64**: ~25-35 MB
- **Windows x64**: ~20-30 MB
- **macOS x64**: ~25-35 MB
- **macOS ARM64**: ~25-35 MB

*Sizes may vary based on dependencies and trimming settings.*

## ⚠️ Troubleshooting

### Build Errors

**"AOT analysis warnings"**
- Check TrimmerRoots.xml includes all necessary types
- Ensure no dynamic type loading in code

**"Missing native dependencies"**
- Install required build tools for your platform
- On Linux: `sudo apt-get install clang zlib1g-dev`

### Runtime Issues

**"Type not found" errors**
- Add the type to TrimmerRoots.xml
- Ensure all ViewModels are registered in DI container

**Asset loading failures**
- Verify assets use `AvaloniaResource` build action
- Check asset paths are correct

## 🧪 Testing the Build

Before distribution, test the published binary:

```bash
# Linux
cd ProductivityCake/bin/Release/net9.0/linux-x64/publish/
./ProductivityCake

# Windows
cd ProductivityCake\bin\Release\net9.0\win-x64\publish\
ProductivityCake.exe

# macOS
cd ProductivityCake/bin/Release/net9.0/osx-x64/publish/
./ProductivityCake
```

## 📚 Additional Resources

- [.NET Native AOT Documentation](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Avalonia Deployment Guide](https://docs.avaloniaui.net/docs/deployment/native-aot)
- [Platform Support Matrix](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/#platformarchitecture-restrictions)

## 🎯 Quick Start

For Linux (current platform):

```bash
# Make scripts executable
chmod +x publish-*.sh

# Build for Linux
./publish-linux.sh

# Run the application
cd ProductivityCake/bin/Release/net9.0/linux-x64/publish/
./ProductivityCake
```

---

**Note**: First-time Native AOT builds may take longer (5-10 minutes) as the compiler performs ahead-of-time compilation and optimization. Subsequent builds will be faster.
