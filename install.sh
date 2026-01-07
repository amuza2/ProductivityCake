#!/bin/bash

# ProductivityCake Installation Script

set -e

INSTALL_DIR="$HOME/.local/bin"
SHARE_DIR="$HOME/.local/share"
APP_NAME="ProductivityCake"

echo "🍰 Installing ProductivityCake..."
echo ""

# Create directories
mkdir -p "$INSTALL_DIR"
mkdir -p "$SHARE_DIR/applications"
mkdir -p "$SHARE_DIR/icons/hicolor/96x96/apps"
mkdir -p "$SHARE_DIR/$APP_NAME"

# Copy binary and dependencies
echo "📋 Installing application files to $SHARE_DIR/$APP_NAME..."
cp "$APP_NAME" "$SHARE_DIR/$APP_NAME/"
if ls *.so 1> /dev/null 2>&1; then
    echo "📦 Installing shared libraries..."
    cp *.so "$SHARE_DIR/$APP_NAME/"
fi
chmod +x "$SHARE_DIR/$APP_NAME/$APP_NAME"

# Copy sound files
if [ -f "alarm.mp3" ]; then
    echo "🔔 Installing sound files..."
    cp *.mp3 "$SHARE_DIR/$APP_NAME/"
    echo "✅ Sound files installed"
fi

# Create symlink in bin
echo "🔗 Creating symlink in $INSTALL_DIR..."
ln -sf "$SHARE_DIR/$APP_NAME/$APP_NAME" "$INSTALL_DIR/$APP_NAME"

# Copy icon to multiple sizes
if [ -f "icons8-cake-96.png" ]; then
    echo "🎨 Installing application icon..."
    for size in 48 64 96 128 256; do
        mkdir -p "$SHARE_DIR/icons/hicolor/${size}x${size}/apps"
        cp icons8-cake-96.png "$SHARE_DIR/icons/hicolor/${size}x${size}/apps/productivitycake.png"
    done
    echo "✅ Icon installed in multiple sizes"
fi

# Create desktop entry
echo "📝 Creating desktop entry..."
cat > "$SHARE_DIR/applications/productivitycake.desktop" << 'DESKTOP'
[Desktop Entry]
Version=1.1
Type=Application
Name=ProductivityCake
Comment=A simple and elegant Pomodoro timer with project management
Exec=ProductivityCake
Icon=productivitycake
Terminal=false
Keywords=pomodoro;timer;productivity;focus;work;
Categories=Utility;Office;ProjectManagement;
StartupNotify=true
DESKTOP

# Update desktop database
if command -v update-desktop-database &> /dev/null; then
    update-desktop-database "$SHARE_DIR/applications" 2>/dev/null || true
fi

# Update icon cache
if command -v gtk-update-icon-cache &> /dev/null; then
    gtk-update-icon-cache -f -t "$SHARE_DIR/icons/hicolor" 2>/dev/null || true
fi

# Add to PATH if not already there
if [[ ":$PATH:" != *":$INSTALL_DIR:"* ]]; then
    echo ""
    echo "⚠️  Add $INSTALL_DIR to your PATH by adding this to ~/.bashrc or ~/.zshrc:"
    echo "   export PATH=\"\$HOME/.local/bin:\$PATH\""
fi

echo ""
echo "✅ Installation complete!"
echo ""
echo "📁 Installation location: $SHARE_DIR/$APP_NAME"
echo "🚀 To run: $APP_NAME"
echo ""
echo "💡 You can also find ProductivityCake in your application menu."
echo ""
