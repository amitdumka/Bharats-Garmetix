using CommunityToolkit.Mvvm.Input;
using Paggar.Models;

namespace Paggar.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}