using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Garmetix.Core.PageModels
{
    public interface IPageModel<TEntity>
    {
        string Title { get; }
        bool IsBusy { get; }
        bool IsDataLoading { get; }
        bool EnableAdd { get; }
        ObservableCollection<TEntity> Entities { get; }
        ICommand LoadInitialDataCommand { get; }
        ICommand LoadMoreDataCommand { get; }
        ICommand AddButtonCommand { get; }
    }
}