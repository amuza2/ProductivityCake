<div align="center">

![Productivity Cake Logo](ProductivityCake/Assets/icons8-cake-96.png)

# ProductivityCake

**A beautiful Pomodoro timer with project management**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3-8B5CF6)](https://avaloniaui.net/)
[![License](https://img.shields.io/badge/License-GPL--3.0-blue.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.2.1-green)](https://github.com/amuza2/ProductivityCake/releases)

</div>

---

## 📸 Screenshots

<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/618aa6cc-adae-439c-a4fc-a724182ad0c2" />
<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/4bd4f033-20f1-40c3-b87a-5e6089ac743d" />
<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/16dd7efd-51bd-4832-8dc6-5da75fee36f4" />
<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/3e16c534-69ab-466d-a24f-f8eed4e605ac" />
<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/1e41aff0-9a28-44e5-afe8-eafa69d519ad" />
<img width="30%" height="40%" alt="image" src="https://github.com/user-attachments/assets/085b1ab0-8667-4d0c-aaa3-07772d1ec6f3" />


<img width="20%" height="30%" alt="image" src="https://github.com/user-attachments/assets/2297a9b5-8dfb-4fbc-8554-8b746fc194bf" />


## ✨ Features

### ⏱️ Pomodoro Timer
- Customizable work sessions (1-90 minutes, default 25 min)
- Smart breaks (5 min short, 15 min long)
- Visual circular progress bar

### 🔔 Notification System
- Ajust notification sounds (Alarm, Bird Tweet)
- Adjustable volume control (0-100%)
- Customizable notification timeout (5-60 seconds)
- Desktop notifications with configurable duration

### 📁 Project & Task Management
- Create projects with descriptions
- Kanban board (ToDo, Doing, Done)
- Archive completed projects
- Task CRUD operations with due dates


## 📥 Installation

Download from [Releases](https://github.com/amuza2/ProductivityCake/releases)

**Linux:**
```bash
# AppImage (recommended)
chmod +x ProductivityCake-*.AppImage
./ProductivityCake-*.AppImage

# Or standalone binary with installer
tar -xzf ProductivityCake-linux-x64.tar.gz
./install.sh
```

**Windows:** (not available yet)
- Extract `ProductivityCake-win-x64.zip`
- Run `ProductivityCake.exe`

---

## 🛠️ Building from Source

**Prerequisites:** [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```bash
# Clone and run
git clone https://github.com/amuza2/ProductivityCake.git
cd ProductivityCake
dotnet run --project ProductivityCake/ProductivityCake.csproj

# Publish (supports Linux, Windows, macOS)
chmod +x publish.sh
./publish.sh

# Build AppImage (Linux only)
chmod +x build-appimage.sh
./build-appimage.sh
```

**Linux dependencies:**
```bash
sudo apt install clang zlib1g-dev libnotify-bin fuse libfuse2
```

## 🏗️ Tech Stack

- Avalonia UI 11.3 • .NET 10.0 • MVVM • Native AOT
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
