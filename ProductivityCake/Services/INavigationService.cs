using System.Threading.Tasks;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Services;

public interface INavigationService
{
    Task NavigateToAsync<T>() where T : ViewModelBase;
}