<div align="center">

![Productivity Cake Logo](ProductivityCake/Assets/icons8-cake-96.png)

**Your Productivity Companion**

A modern, lightweight desktop application for managing projects, tasks, and time using the Pomodoro technique.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.0-8B5CF6)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-GPL--3.0-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Linux%20%7C%20Windows%20%7C%20macOS-lightgrey)](https://github.com/amuza2/ProductivityCake)
[![CI](https://github.com/amuza2/ProductivityCake/actions/workflows/ci.yml/badge.svg)](https://github.com/amuza2/ProductivityCake/actions/workflows/ci.yml)

[Features](#-features) • [Installation](#-installation)  • [Screenshots](#-screenshots) • [Building](#-building-from-source)

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
- **Task Details** - Add descriptions, due dates, and assign to projects
- **Status Tracking** - Track progress with ToDo, Doing, and Done states

### ⏱️ Pomodoro Timer
- **25-50 Minute Work Sessions** - Focused work periods for maximum productivity
- **Smart Breaks** - 5-minute short breaks and 15-minute long breaks
- **Auto-Advance** - Automatically transitions between work and break sessions
- **Progress Tracking** - Visual circular progress bar and completed pomodoro counter
- **Flexible Controls** - Start, pause, reset, skip, and switch modes manually
- **Time Adjustment** - Add or subtract 5 minutes during active sessions
- **Audio Notifications** - Sound alerts when timer completes
- **Customizable Settings** - Adjust work and break durations (1-90 minutes)

### 📊 Statistics & Analytics
- **Daily Statistics** - Track today's work time, break time, and completed sessions
- **Weekly & Monthly Stats** - View aggregated time worked over different periods
- **GitHub-Style Heatmap** - Visualize your entire year of productivity at a glance
  - Color-coded intensity based on daily session counts
  - Day labels (Mon, Wed, Fri) on Y-axis
  - Month labels on X-axis
  - Hover tooltips showing date and session count
- **Real-time Updates** - Statistics update automatically as you complete sessions

### ⚙️ Settings & Customization
- **Timer Configuration** - Customize work, short break, and long break durations
- **Notification Controls** - Toggle desktop notifications on/off
- **Always on Top** - Keep the window above all other applications
- **Test Notifications** - 5-second test timer to verify sound and notifications

---

## 📥 Installation

### Download Pre-built Binaries

**All platforms will be available in the [Releases](https://github.com/amuza2/ProductivityCake/releases) section.**

### Linux

**Option 1: AppImage (Recommended - Works on all distros)**

1. Download `ProductivityCake-x.x.x-x86_64.AppImage` from [Releases](https://github.com/amuza2/ProductivityCake/releases)
2. Make it executable and run:
   ```bash
   chmod +x ProductivityCake-*.AppImage
   ./ProductivityCake-*.AppImage
   ```

**Option 2: Standalone Binary**

1. Download `ProductivityCake-linux-x64.tar.gz` from [Releases](https://github.com/amuza2/ProductivityCake/releases)
2. Extract and run:
   ```bash
   tar -xzf ProductivityCake-linux-x64.tar.gz
   ./ProductivityCake
   ```

### Windows

1. Download `ProductivityCake-win-x64.zip` from [Releases](https://github.com/amuza2/ProductivityCake/releases)
2. Extract the ZIP file
3. Run `ProductivityCake.exe`

---

## 📸 Screenshots

<img width="400" height="600" alt="image" src="https://github.com/user-attachments/assets/25f14f69-6f93-444b-aeb6-ba410d6915d2" />

<img width="400" height="600" alt="image" src="https://github.com/user-attachments/assets/d12b8efe-7086-45d4-86b6-efba3cd52a33" />

<img width="400" height="600" alt="image" src="https://github.com/user-attachments/assets/6e668f05-703a-4dc7-b6a8-d1f6471d9878" />

<img width="400" height="600" alt="image" src="https://github.com/user-attachments/assets/b617336b-2c4f-431d-a1fe-c0832b7265a7" />

<img width="400" height="600" alt="image" src="https://github.com/user-attachments/assets/77e818b2-8f06-47c1-8e8b-9ebabc66e308" />


---

## 🛠️ Building from Source

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Linux**: 
  - `clang` and `zlib1g-dev` (for AOT compilation)
  - `libnotify-bin` (for desktop notifications)
  ```bash
  sudo apt install clang zlib1g-dev libnotify-bin
  ```

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

### Publish for Linux (Native AOT)

**Option 1: Using the publish script (recommended)**
```bash
chmod +x publish-linux.sh
./publish-linux.sh
```

**Option 2: Manual publish**
```bash
# Publish optimized native binary for Linux x64
dotnet publish ProductivityCake/ProductivityCake.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained \
  -o ./publish/linux-x64

# The executable will be at: ./publish/linux-x64/ProductivityCake
# Create a distributable archive
cd publish/linux-x64
tar -czf ProductivityCake-linux-x64.tar.gz ProductivityCake alarm.mp3
```

### Build AppImage (Universal Linux Package)

**Prerequisites for AppImage:**
```bash
# On Arch/EndeavourOS
sudo pacman -S fuse2

# On Ubuntu/Debian
sudo apt install fuse libfuse2
```

**Build the AppImage:**
```bash
chmod +x build-appimage.sh
./build-appimage.sh
```

This creates a universal `ProductivityCake-1.1.0-x86_64.AppImage` that works on:
- ✅ Arch Linux / EndeavourOS / Manjaro
- ✅ Ubuntu / Debian / Linux Mint
- ✅ Fedora / RHEL / CentOS
- ✅ openSUSE
- ✅ Any Linux distro with FUSE support


## 🏗️ Technology Stack

- **Framework**: [Avalonia UI 11.0](https://avaloniaui.net/) - Cross-platform .NET UI framework
- **Language**: C# / .NET 9.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **UI Toolkit**: [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- **Data Storage**: JSON file-based storage with source generation

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

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

This means you are free to use, modify, and distribute this software, but any modifications must also be released under the GPL-3.0 license.

---

## 🙏 Acknowledgments

- Icons by [Icons8](https://icons8.com/)
- Built with [Avalonia UI](https://avaloniaui.net/)
- Inspired by the Pomodoro Technique by Francesco Cirillo

---

<div align="center">

**Made with ❤️ and ☕**

If you find this project helpful, please consider giving it a ⭐!

</div>
