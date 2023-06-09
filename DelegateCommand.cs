using System;
using System.Threading.Tasks;

namespace Meziantou.Framework.WPF;

public static class DelegateCommand
{
    public static IDelegateCommand Create(Action? execute) => new SyncDelegateCommand(WrapAction(execute), CanExecuteTrue);

    public static IDelegateCommand Create(Action<object?>? execute) => new SyncDelegateCommand(execute ?? DefaultExecute, CanExecuteTrue);

    public static IDelegateCommand Create(Action? execute, Func<bool>? canExecute) => new SyncDelegateCommand(WrapAction(execute), WrapAction(canExecute));

    public static IDelegateCommand Create(Action<object?>? execute, Func<object?, bool>? canExecute) => new SyncDelegateCommand(execute ?? DefaultExecute, canExecute ?? CanExecuteTrue);

    public static IDelegateCommand Create(Func<Task>? execute) => new AsyncDelegateCommand(WrapAction(execute), CanExecuteTrue);

    public static IDelegateCommand Create(Func<object?, Task>? execute) => new AsyncDelegateCommand(execute ?? DefaultExecuteAsync, CanExecuteTrue);

    public static IDelegateCommand Create(Func<Task>? execute, Func<bool>? canExecute) => new AsyncDelegateCommand(WrapAction(execute), WrapAction(canExecute));

    public static IDelegateCommand Create(Func<object?, Task>? execute, Func<object?, bool>? canExecute) => new AsyncDelegateCommand(execute ?? DefaultExecuteAsync, canExecute ?? CanExecuteTrue);

    private static void DefaultExecute(object? _)
    {
    }

    private static Task DefaultExecuteAsync(object? _)
    {
        #if NET452
        return Task.FromResult(false);
        #else
        return Task.CompletedTask;
        #endif
    }

    private static bool CanExecuteTrue(object? _) => true;

    private static Func<object?, Task> WrapAction(Func<Task>? action)
    {
        if (action == null)
            return DefaultExecuteAsync;

        return _ => action();
    }

    private static Action<object?> WrapAction(Action? action)
    {
        if (action == null)
            return DefaultExecute;

        return _ => action();
    }

    private static Func<object?, bool> WrapAction(Func<bool>? action)
    {
        if (action == null)
            return CanExecuteTrue;

        return _ => action();
    }
}
