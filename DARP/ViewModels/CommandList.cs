namespace DARP.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class CommandList
    {
        public DelegateCommand AddCommand(Action execute, Func<bool> canExecute)
        {
            Action executeAndUpdate = execute + Update;
            DelegateCommand command = new DelegateCommand(executeAndUpdate, canExecute);
            notifyAll += command.NotifyCanExecuteChanged;

            return command;
        }

        public DelegateCommand AddAsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            Action executeAndUpdate = async () =>
            {
                await execute();
                Update();
            };

            DelegateCommand command = new DelegateCommand(executeAndUpdate, canExecute);
            notifyAll += command.NotifyCanExecuteChanged;

            return command;
        }

        public DelegateCommand<T> AddGenericCommand<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            Action<T> executeAndUpdate = (param) =>
            {
                execute(param);
                Update();
            };

            DelegateCommand<T> command = new DelegateCommand<T>(executeAndUpdate, canExecute);
            notifyAll += command.NotifyCanExecuteChanged;

            return command;
        }

        public void Update()
        {
            notifyAll?.Invoke();
        }

        public void Clear()
        {
            notifyAll = null;
        }

        private Action notifyAll;

        internal ICommand AddCommand(ICommand addThickness, Func<bool> canAddThickness)
        {
            throw new NotImplementedException();
        }
    }

}
