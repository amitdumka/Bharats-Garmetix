using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Bharat.ToolKits.Extensions
{
    public static class ICommandExtensions
    {
        public static void AttemptNotifyCanExecuteChanged<TCommand>(this TCommand command)
            where TCommand : ICommand
        {
            if (command is IRelayCommand rc)
            {
                rc?.NotifyCanExecuteChanged();
            }
        }
    }
}
