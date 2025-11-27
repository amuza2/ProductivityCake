<div align="center">

![Productivity Cake Logo](ProductivityCake/Assets/icons8-cake-96.png)

# ProductivityCake

**A beautiful Pomodoro timer with project management**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3-8B5CF6)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-GPL--3.0-blue.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.2.0-green)](https://github.com/amuza2/ProductivityCake/releases)

Modern glassmorphism UI • Native AOT • Cross-platform

</div>

---

## ✨ Features

### ⏱️ Pomodoro Timer
- Customizable work sessions (1-90 minutes, default 25 min)
- Smart breaks (5 min short, 15 min long)
- Auto-advance between work and breaks
- Visual circular progress bar
- Audio notifications with sound alerts
- Flexible controls: start, pause, reset, skip

### 📁 Project & Task Management
- Create projects with descriptions
- Kanban board (ToDo, Doing, Done)
- Archive completed projects
- Task CRUD operations with due dates

### 📊 Statistics & Analytics
- Daily, weekly, and monthly time tracking
- GitHub-style activity heatmap
- Session count tracking
- Real-time statistics updates

### 🎨 Modern UI
- Beautiful glassmorphism design
- Smooth animations and transitions
- Interactive hover effects
- Always-on-top mode

---

## 📥 Installation

Download from [Releases](https://github.com/amuza2/ProductivityCake/releases)

**Linux:**
```bash
# AppImage (recommended)
chmod +x ProductivityCake-*.AppImage
./ProductivityCake-*.AppImage

# Or standalone binary
tar -xzf ProductivityCake-linux-x64-v1.2.0.tar.gz
./ProductivityCake
```

**Windows:**
- Extract `ProductivityCake-win-x64.zip`
- Run `ProductivityCake.exe`

---

## 🛠️ Building from Source

**Prerequisites:** [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

```bash
# Clone and run
git clone https://github.com/amuza2/ProductivityCake.git
cd ProductivityCake
dotnet run --project ProductivityCake/ProductivityCake.csproj

# Publish for Linux (Native AOT)
chmod +x publish-linux.sh
./publish-linux.sh

# Build AppImage
chmod +x build-appimage.sh
./build-appimage.sh
```

**Linux dependencies:**
```bash
sudo apt install clang zlib1g-dev libnotify-bin fuse libfuse2
```

## 🏗️ Tech Stack

- Avalonia UI 11.3 • .NET 9.0 • MVVM • Native AOT
- CommunityToolkit.Mvvm • JSON storage

---

## 📄 License

GPL-3.0 License - see [LICENSE](LICENSE) for details.

---

<div align="center">

**Made with ❤️ by [amuza2](https://github.com/amuza2)**

Icons by [Icons8](https://icons8.com/) • Built with [Avalonia UI](https://avaloniaui.net/)

If you find this helpful, give it a ⭐!

</div>
