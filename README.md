# ProductivityCake

A comprehensive productivity tool built with Avalonia UI for managing projects, tasks, and time using the Pomodoro technique.

## Features

###  Project Management
- **Create Projects**: Add new projects with names and descriptions
- **View Projects**: See all your projects in a clean, organized list
- **Delete Projects**: Remove projects you no longer need
- **Project-Task Association**: Link tasks to specific projects

###  Task Management (CRUD)
- **Create Tasks**: Add new tasks with titles, descriptions, and due dates
- **Read Tasks**: View all your tasks in an organized list
- **Update Tasks**: Edit task details (coming soon)
- **Delete Tasks**: Remove completed or unnecessary tasks
- **Task Filtering**: Filter tasks by completion status
- **Project Assignment**: Assign tasks to specific projects

###  Pomodoro Timer
- **Work Sessions**: 25-minute focused work periods
- **Short Breaks**: 5-minute breaks between work sessions
- **Long Breaks**: 15-minute breaks after 4 completed pomodoros
- **Auto-Advance**: Automatically transitions between work and break sessions
- **Progress Tracking**: Visual progress bar and completed pomodoro counter
- **Timer Controls**: Start, pause, and reset functionality
- **Mode Selection**: Manually switch between work, short break, and long break modes

## How to Use

### Running the Application
```bash
dotnet run --project ProductivityCake
```

### Navigation
The application has three main tabs accessible from the bottom navigation bar:
- **Projects** (): Manage your projects
- **Tasks** (): View and manage your tasks
- **Timer** (): Use the Pomodoro timer for focused work

### Pomodoro Workflow
1. Start a work session (25 minutes)
2. Take a short break (5 minutes) after each work session
3. After 4 work sessions, take a long break (15 minutes)
4. Repeat the cycle

## Technology Stack
- **Framework**: Avalonia UI (Cross-platform .NET UI framework)
- **Language**: C# / .NET 9.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **Data Storage**: JSON file-based storage
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

## Project Structure
```
ProductivityCake/
Models/          # Data models (Project, TodoItem)
ViewModels/      # View models for MVVM pattern
Views/           # UI views (AXAML files)
Services/        # Business logic services
Converters/      # Value converters for data binding
Assets/          # Icons and resources
```

## Future Enhancements
- Task editing functionality
- Task priority levels
- Project statistics and analytics
- Notifications when timer completes
- Task search functionality
- Export/import data
- Cloud synchronization productivity application
