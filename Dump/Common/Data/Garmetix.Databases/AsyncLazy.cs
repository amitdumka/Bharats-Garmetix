using System.Runtime.CompilerServices;
namespace Garmetix.Databases;

/// <summary>
/// Async Lazy : Lazy Initiliztion
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncLazy<T>
{
    readonly Lazy<Task<T>> instance;

    public AsyncLazy(Func<T> factory)
    {
        instance = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    public AsyncLazy(Func<Task<T>> factory)
    {
        instance = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    public TaskAwaiter<T> GetAwaiter()
    {
        return instance.Value.GetAwaiter();
    }

    public void Start()
    {
        _ = instance.Value;
    }
}