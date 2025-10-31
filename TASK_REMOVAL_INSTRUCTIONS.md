# Task Tab Removal - Remaining Steps

## ✅ Completed Changes

1. **Fixed project loading issue** - Changed `ProjectService` to load projects synchronously in constructor
2. **Removed Tasks navigation** - Updated `MainWindowViewModel` to remove `NavigateToTasksPage` command
3. **Updated default navigation** - App now opens on Projects tab instead of Tasks
4. **Simplified `ProjectDetailsViewModel`** - Removed all task-related code
5. **Simplified `ProjectDetailsView.axaml`** - Removed all task UI elements
6. **Updated `MainWindow.axaml`** - Removed Tasks button from footer (now only Projects and Timer)
7. **Removed task registrations** - Updated `ServiceCollectionExtensions.cs`
8. **Removed Tasks collection** - Updated `Project.cs` model

## ⚠️ Files to Delete Manually

Please delete the following files from your project:

### Views (6 files):
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskListView.axaml`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskListView.axaml.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskDetailsView.axaml`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskDetailsView.axaml.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskCreationView.axaml`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Views/TaskCreationView.axaml.cs`

### ViewModels (3 files):
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/ViewModels/TaskListViewModel.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/ViewModels/TaskDetailsViewModel.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/ViewModels/TaskCreationViewModel.cs`

### Models (3 files):
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Models/TodoItem.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Models/TaskStatus.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Models/RepeatType.cs`

### Services (3 files):
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Services/ITodoService.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Services/JsonDataService.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Services/IJsonDataService.cs`

### Converters (2 files):
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Converters/DueDateConverter.cs`
- `/home/mohammed/RiderProjects/ProductivityCake/ProductivityCake/Converters/DueDateToColorConverter.cs`

## 🔧 How to Delete Files in Rider

1. In the Solution Explorer, select the files listed above
2. Right-click and choose "Delete" or press Delete key
3. Confirm the deletion

## ✅ Testing After Deletion

After deleting the files, rebuild the project and verify:

1. **Build succeeds** - No compilation errors
2. **App starts on Projects tab** - Default view is Projects
3. **Footer shows only 2 tabs** - Projects (folder icon) and Timer (clock icon)
4. **Navigation works** - Can switch between Projects and Timer
5. **Projects load immediately** - No need to navigate away and back
6. **Project details view works** - Can click on a project to see its details

## 🐛 Issues Fixed

### Issue #1: Projects not showing until navigating away and back
**Root Cause**: `ProjectService` constructor was calling `LoadProjects().ConfigureAwait(false)` without awaiting, so the constructor returned before projects were loaded.

**Fix**: Changed to synchronous loading in constructor using `LoadProjectsSync()` method that uses `File.ReadAllText()` instead of async version.

## 📝 Notes

- The CategoryService is still registered and functional (used for project categorization)
- Timer functionality remains unchanged
- All task-related data files (todo.json) will remain on disk but won't be accessed
