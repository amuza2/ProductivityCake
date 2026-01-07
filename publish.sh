#!/bin/bash

# ProductivityCake Publishing Script
# This script publishes the app for different platforms

echo "🎂 ProductivityCake Publishing Script"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

PROJECT_DIR="ProductivityCake"
OUTPUT_DIR="publish"

# Create output directory
mkdir -p $OUTPUT_DIR

echo -e "${BLUE}Select platform to publish:${NC}"
echo "1) Linux (x64)"
echo "2) Windows (x64)"
echo "3) macOS (x64)"
echo "4) macOS (ARM64)"
echo "5) All platforms"
echo ""
read -p "Enter choice [1-5]: " choice

publish_linux() {
    echo -e "${GREEN}Publishing for Linux x64...${NC}"
    dotnet publish $PROJECT_DIR/ProductivityCake.csproj \
        -c Release \
        -r linux-x64 \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        -p:PublishAot=true \
        -o $OUTPUT_DIR/linux-x64
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Linux build completed!${NC}"
        
        # Copy assets to output directory
        echo -e "${BLUE}Copying assets...${NC}"
        cp $PROJECT_DIR/Assets/alarm.mp3 $OUTPUT_DIR/linux-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/9326__tigersound__bird-tweet-3.mp3 $OUTPUT_DIR/linux-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/icons8-cake-96.png $OUTPUT_DIR/linux-x64/ 2>/dev/null || true
        cp install.sh $OUTPUT_DIR/linux-x64/ 2>/dev/null || true
        
        # Create distributable archive
        echo -e "${BLUE}Creating distributable archive...${NC}"
        cd $OUTPUT_DIR/linux-x64
        tar -czf ProductivityCake-linux-x64.tar.gz ProductivityCake *.so *.mp3 *.png install.sh 2>/dev/null || \
        tar -czf ProductivityCake-linux-x64.tar.gz ProductivityCake *.so
        cd ../..
        
        echo -e "${GREEN}✓ Archive created: $OUTPUT_DIR/linux-x64/ProductivityCake-linux-x64.tar.gz${NC}"
        echo -e "Output: $OUTPUT_DIR/linux-x64/"
    else
        echo -e "${YELLOW}⚠ Linux build failed${NC}"
    fi
}

publish_windows() {
    echo -e "${GREEN}Publishing for Windows x64...${NC}"
    dotnet publish $PROJECT_DIR/ProductivityCake.csproj \
        -c Release \
        -r win-x64 \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        -p:PublishAot=true \
        -o $OUTPUT_DIR/win-x64
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Windows build completed!${NC}"
        
        # Copy assets to output directory
        echo -e "${BLUE}Copying assets...${NC}"
        cp $PROJECT_DIR/Assets/alarm.mp3 $OUTPUT_DIR/win-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/9326__tigersound__bird-tweet-3.mp3 $OUTPUT_DIR/win-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/icons8-cake-96.png $OUTPUT_DIR/win-x64/ 2>/dev/null || true
        
        # Create distributable archive
        echo -e "${BLUE}Creating distributable archive...${NC}"
        cd $OUTPUT_DIR/win-x64
        zip -q ProductivityCake-win-x64.zip ProductivityCake.exe *.mp3 *.png 2>/dev/null || \
        zip -q ProductivityCake-win-x64.zip ProductivityCake.exe
        cd ../..
        
        echo -e "${GREEN}✓ Archive created: $OUTPUT_DIR/win-x64/ProductivityCake-win-x64.zip${NC}"
        echo -e "Output: $OUTPUT_DIR/win-x64/"
    else
        echo -e "${YELLOW}⚠ Windows build failed${NC}"
    fi
}

publish_macos_x64() {
    echo -e "${GREEN}Publishing for macOS x64...${NC}"
    dotnet publish $PROJECT_DIR/ProductivityCake.csproj \
        -c Release \
        -r osx-x64 \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        -p:PublishAot=true \
        -o $OUTPUT_DIR/osx-x64
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ macOS x64 build completed!${NC}"
        
        # Copy assets to output directory
        echo -e "${BLUE}Copying assets...${NC}"
        cp $PROJECT_DIR/Assets/alarm.mp3 $OUTPUT_DIR/osx-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/9326__tigersound__bird-tweet-3.mp3 $OUTPUT_DIR/osx-x64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/icons8-cake-96.png $OUTPUT_DIR/osx-x64/ 2>/dev/null || true
        
        # Create distributable archive
        echo -e "${BLUE}Creating distributable archive...${NC}"
        cd $OUTPUT_DIR/osx-x64
        tar -czf ProductivityCake-osx-x64.tar.gz ProductivityCake *.mp3 *.png 2>/dev/null || \
        tar -czf ProductivityCake-osx-x64.tar.gz ProductivityCake
        cd ../..
        
        echo -e "${GREEN}✓ Archive created: $OUTPUT_DIR/osx-x64/ProductivityCake-osx-x64.tar.gz${NC}"
        echo -e "Output: $OUTPUT_DIR/osx-x64/"
    else
        echo -e "${YELLOW}⚠ macOS x64 build failed${NC}"
    fi
}

publish_macos_arm64() {
    echo -e "${GREEN}Publishing for macOS ARM64...${NC}"
    dotnet publish $PROJECT_DIR/ProductivityCake.csproj \
        -c Release \
        -r osx-arm64 \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        -p:PublishAot=true \
        -o $OUTPUT_DIR/osx-arm64
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ macOS ARM64 build completed!${NC}"
        
        # Copy assets to output directory
        echo -e "${BLUE}Copying assets...${NC}"
        cp $PROJECT_DIR/Assets/alarm.mp3 $OUTPUT_DIR/osx-arm64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/9326__tigersound__bird-tweet-3.mp3 $OUTPUT_DIR/osx-arm64/ 2>/dev/null || true
        cp $PROJECT_DIR/Assets/icons8-cake-96.png $OUTPUT_DIR/osx-arm64/ 2>/dev/null || true
        
        # Create distributable archive
        echo -e "${BLUE}Creating distributable archive...${NC}"
        cd $OUTPUT_DIR/osx-arm64
        tar -czf ProductivityCake-osx-arm64.tar.gz ProductivityCake *.mp3 *.png 2>/dev/null || \
        tar -czf ProductivityCake-osx-arm64.tar.gz ProductivityCake
        cd ../..
        
        echo -e "${GREEN}✓ Archive created: $OUTPUT_DIR/osx-arm64/ProductivityCake-osx-arm64.tar.gz${NC}"
        echo -e "Output: $OUTPUT_DIR/osx-arm64/"
    else
        echo -e "${YELLOW}⚠ macOS ARM64 build failed${NC}"
    fi
}

case $choice in
    1)
        publish_linux
        ;;
    2)
        publish_windows
        ;;
    3)
        publish_macos_x64
        ;;
    4)
        publish_macos_arm64
        ;;
    5)
        echo -e "${BLUE}Publishing for all platforms...${NC}"
        echo ""
        publish_linux
        echo ""
        publish_windows
        echo ""
        publish_macos_x64
        echo ""
        publish_macos_arm64
        ;;
    *)
        echo -e "${YELLOW}Invalid choice${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${GREEN}======================================"
echo -e "Publishing complete! 🎉${NC}"
echo ""
echo "Published files are in the '$OUTPUT_DIR' directory"
echo ""
echo "File sizes:"
du -sh $OUTPUT_DIR/* 2>/dev/null || echo "No builds found"
