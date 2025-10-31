# 🍰 Productivity Cake

<div align="center">

![Productivity Cake Logo](ProductivityCake/Assets/icons8-cake-96.png)

**Your Productivity Companion**

A modern, lightweight desktop application for managing projects, tasks, and time using the Pomodoro technique.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.0-8B5CF6)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Linux%20%7C%20Windows%20%7C%20macOS-lightgrey)](https://github.com/amuza2/ProductivityCake)

[Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Screenshots](#-screenshots) • [Building](#-building-from-source)

</div>

---

## ✨ Features

### 📁 Project Management
- **Create & Organize Projects** - Add projects with names and descriptions
- **Kanban Board View** - Visualize tasks in ToDo, Doing, and Done columns
- **Archive Projects** - Keep your workspace clean without losing data
- **Task Association** - Link tasks to specific projects for better organization

### ✅ Task Management
- **Full CRUD Operations** - Create, read, update, and delete tasks
- **Drag & Drop** - Move tasks between status columns effortlessly
- **Task Details** - Add descriptions, due dates, and assign to projects
- **Category System** - Organize tasks with customizable categories
- **Status Tracking** - Track progress with ToDo, Doing, and Done states

### ⏱️ Pomodoro Timer
- **25-Minute Work Sessions** - Focused work periods for maximum productivity
- **Smart Breaks** - 5-minute short breaks and 15-minute long breaks
- **Auto-Advance** - Automatically transitions between work and break sessions
- **Progress Tracking** - Visual progress bar and completed pomodoro counter
- **Flexible Controls** - Start, pause, reset, and switch modes manually
- **Audio Notifications** - Sound alerts when timer completes

### 🎨 Modern UI
- **Custom Title Bar** - Borderless window with minimize and close controls
- **System Tray Integration** - Minimize to tray and keep running in background
- **Vibrant Color Scheme** - Beautiful purple-themed interface
- **Smooth Animations** - Polished hover effects and transitions
- **Responsive Design** - Clean, intuitive layout optimized for desktop

### 🚀 Performance
- **Native AOT Compilation** - Fast startup and low memory footprint
- **Self-Contained** - No .NET runtime installation required
- **Lightweight** - ~25-35 MB binary size
- **Cross-Platform** - Runs on Linux, Windows, and macOS

---

## 📥 Installation

### Linux

**Download & Run:**
```bash
# Download the latest release
wget https://github.com/amuza2/ProductivityCake/releases/latest/download/ProductivityCake-linux-x64.tar.gz

# Extract
tar -xzf ProductivityCake-linux-x64.tar.gz

# Run
./ProductivityCake
```

**Quick Install:**
```bash
# Install to ~/.local/share
./install.sh

# Run from anywhere
productivitycake
```

### Windows
Download `ProductivityCake-win-x64.zip` from [Releases](https://github.com/amuza2/ProductivityCake/releases) and extract.

### macOS
Download `ProductivityCake-osx-x64.tar.gz` or `ProductivityCake-osx-arm64.tar.gz` from [Releases](https://github.com/amuza2/ProductivityCake/releases).

---

## 🎮 Usage

### Getting Started

1. **Launch the Application**
   - Click the app icon or run from terminal
   - The app starts with a custom title bar and system tray icon

2. **Create Your First Project**
   - Navigate to the Projects tab
   - Click the "+" button
   - Enter project name and description
   - Click "Add Project"

3. **Add Tasks**
   - Open a project to view its Kanban board
   - Click "Add Task" in any column
   - Fill in task details
   - Drag tasks between columns as you progress

4. **Use the Pomodoro Timer**
   - Switch to the Timer tab
   - Click "Start" to begin a 25-minute work session
   - Take breaks when prompted
   - Track your completed pomodoros

### Keyboard Shortcuts & Tips

- **Minimize to Tray**: Click the window close button (X)
- **Restore from Tray**: Click the tray icon
- **Drag Window**: Click and drag the title bar
- **Quick Navigation**: Use the bottom navigation buttons

### System Tray Menu

Right-click the tray icon for:
- **Show/Hide** - Toggle window visibility
- **Go to Repository** - Open GitHub page
- **Exit** - Close the application completely

---

## 📸 Screenshots

> *Coming soon - Add screenshots of your application here*

---

## 🛠️ Building from Source

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Linux: `clang` and `zlib1g-dev` (for AOT compilation)

### Clone & Build

```bash
# Clone the repository
git clone https://github.com/amuza2/ProductivityCake.git
cd ProductivityCake

# Build for development
dotnet build

# Run
dotnet run --project ProductivityCake/ProductivityCake.csproj
```

### Publish for Production

**Linux:**
```bash
./publish-linux.sh
# Output: ProductivityCake/bin/Release/net9.0/linux-x64/publish/
```

**Windows:**
```bash
./publish-windows.sh
# Output: ProductivityCake/bin/Release/net9.0/win-x64/publish/
```

**macOS:**
```bash
./publish-macos.sh
# Output: ProductivityCake/bin/Release/net9.0/osx-x64/publish/
```

---

## 🏗️ Technology Stack

- **Framework**: [Avalonia UI 11.0](https://avaloniaui.net/) - Cross-platform .NET UI framework
- **Language**: C# / .NET 9.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **UI Toolkit**: [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- **Data Storage**: JSON file-based storage with source generation
- **Deployment**: Native AOT compilation for optimal performance

### Project Structure

```
ProductivityCake/
├── Models/              # Data models (Project, TodoItem, Category)
├── ViewModels/          # View models implementing MVVM pattern
├── Views/               # UI views (AXAML files)
├── Services/            # Business logic and data services
├── Converters/          # Value converters for data binding
├── Styles/              # Custom UI styles and themes
└── Assets/              # Icons, images, and resources
```

---

## 🎯 Roadmap

- [ ] Task editing in-place
- [ ] Task priority levels and sorting
- [ ] Project statistics and analytics dashboard
- [ ] Desktop notifications for timer completion
- [ ] Task search and filtering
- [ ] Data export/import (JSON, CSV)
- [ ] Themes support (Dark/Light mode)
- [ ] Keyboard shortcuts
- [ ] Multi-language support
- [ ] Cloud synchronization (optional)

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Icons by [Icons8](https://icons8.com/)
- Built with [Avalonia UI](https://avaloniaui.net/)
- Inspired by the Pomodoro Technique by Francesco Cirillo

---

## 📧 Contact

**Author**: [amuza2](https://github.com/amuza2)

**Repository**: [https://github.com/amuza2/ProductivityCake](https://github.com/amuza2/ProductivityCake)

---

<div align="center">

**Made with ❤️ and ☕**

If you find this project helpful, please consider giving it a ⭐!

</div>
