using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Models;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class ProjectListViewModel : ViewModelBase
{
    private readonly ProjectService? _projectService;
    private readonly INavigationService? _navigationService;
    
    [ObservableProperty]
    private ObservableCollection<Project> _projects = new();
    
    [ObservableProperty]
    private ObservableCollection<Project> _activeProjects = new();
    
    [ObservableProperty]
    private ObservableCollection<Project> _archivedProjects = new();
    
    [ObservableProperty]
    private ObservableCollection<Project> _displayedProjects = new();
    
    public bool HasProjects => Projects.Any();
    
    [ObservableProperty]
    private string _newProjectName = string.Empty;
    
    [ObservableProperty]
    private string? _newProjectDescription;
    
    [ObservableProperty]
    private bool _isAddProjectDialogOpen;
    
    [ObservableProperty]
    private bool _isDeleteConfirmationOpen;
    
    [ObservableProperty]
    private Project? _projectToDelete;
    
    [ObservableProperty]
    private bool _showArchived = false;
    
    public ProjectListViewModel(ProjectService projectService, INavigationService navigationService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _projectService.ProjectsChanged += OnProjectsChanged;
        
        // Load projects on initialization
        _ = LoadProjectsAsync();
    }

    public ProjectListViewModel() : this(null!, null!)
    {
    }
    
    [RelayCommand]
    private async Task LoadProjectsAsync()
    {
        if (_projectService == null) return;
        
        try
        {
            var projects = await _projectService.GetAllProjectsAsync();
            Projects = new ObservableCollection<Project>(projects);
            
            // Update filtered collections
            UpdateFilteredCollections();
            
            Console.WriteLine($"Loaded {projects.Count()} projects. Active: {ActiveProjects.Count}, Archived: {ArchivedProjects.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading projects: {ex.Message}");
        }
    }
    
    private void UpdateFilteredCollections()
    {
        // Update active projects
        ActiveProjects.Clear();
        foreach (var project in Projects.Where(p => !p.IsArchived))
        {
            ActiveProjects.Add(project);
        }
        
        // Update archived projects
        ArchivedProjects.Clear();
        foreach (var project in Projects.Where(p => p.IsArchived))
        {
            ArchivedProjects.Add(project);
        }
        
        // Update displayed projects based on current filter
        UpdateDisplayedProjects();
        
        // Notify HasProjects
        OnPropertyChanged(nameof(HasProjects));
    }
    
    private void UpdateDisplayedProjects()
    {
        DisplayedProjects.Clear();
        var source = ShowArchived ? ArchivedProjects : ActiveProjects;
        foreach (var project in source)
        {
            DisplayedProjects.Add(project);
        }
    }
    
    [RelayCommand]
    private void OpenAddProjectDialog()
    {
        NewProjectName = string.Empty;
        NewProjectDescription = null;
        IsAddProjectDialogOpen = true;
    }
    
    [RelayCommand]
    private void CancelAddProject()
    {
        IsAddProjectDialogOpen = false;
        NewProjectName = string.Empty;
        NewProjectDescription = null;
    }
    
    [RelayCommand(CanExecute = nameof(CanAddProject))]
    private async Task AddProjectAsync()
    {
        if (_projectService == null) return;
        
        try
        {
            var project = new Project
            {
                Name = NewProjectName.Trim(),
                Description = NewProjectDescription?.Trim()
            };
            
            await _projectService.CreateProjectAsync(project);
            
            // Close dialog and clear fields
            IsAddProjectDialogOpen = false;
            NewProjectName = string.Empty;
            NewProjectDescription = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding project: {ex.Message}");
        }
    }
    
    private bool CanAddProject() => !string.IsNullOrWhiteSpace(NewProjectName);
    
    partial void OnNewProjectNameChanged(string value)
    {
        AddProjectCommand.NotifyCanExecuteChanged();
    }
    
    [RelayCommand]
    private void OpenProjectDetails(Project project)
    {
        if (_navigationService == null) return;
        
        _navigationService.NavigateToAsync<ProjectDetailsViewModel>(viewModel =>
        {
            if (viewModel is ProjectDetailsViewModel detailsViewModel)
            {
                detailsViewModel.Initialize(project);
            }
        });
    }
    
    [RelayCommand]
    private void ShowDeleteConfirmation(Project project)
    {
        ProjectToDelete = project;
        IsDeleteConfirmationOpen = true;
    }
    
    [RelayCommand]
    private void CancelDelete()
    {
        IsDeleteConfirmationOpen = false;
        ProjectToDelete = null;
    }
    
    [RelayCommand]
    private async Task ConfirmDeleteAsync()
    {
        if (_projectService == null || ProjectToDelete == null) return;
        
        try
        {
            await _projectService.DeleteProjectAsync(ProjectToDelete.Id);
            IsDeleteConfirmationOpen = false;
            ProjectToDelete = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting project: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task ArchiveProjectAsync(Project project)
    {
        if (_projectService == null) return;
        
        try
        {
            await _projectService.ArchiveProjectAsync(project.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error archiving project: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task UnarchiveProjectAsync(Project project)
    {
        if (_projectService == null) return;
        
        try
        {
            await _projectService.UnarchiveProjectAsync(project.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unarchiving project: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private void ToggleShowArchived()
    {
        ShowArchived = !ShowArchived;
        UpdateDisplayedProjects();
    }
    
    private void OnProjectsChanged(object? sender, EventArgs e)
    {
        // Ensure we're on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
        {
            await LoadProjectsAsync();
        });
    }
}
