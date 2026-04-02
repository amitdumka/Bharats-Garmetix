using CommunityToolkit.Mvvm.Input;
using Garmetix.Models;

namespace Garmetix.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}