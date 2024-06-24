using AsyncToolbox.Utilities;

namespace AsyncToolbox;

public class PendingAwaiter(Func<Action, Action?>? wrapper = null) : IAwaiter
{
    private Action? _continuation;

    private ManualResetEventSlim? _lock;

    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
        if (wrapper != null)
            _continuation = wrapper(continuation);
    }

    public void UnsafeOnCompleted(Action continuation)
        => OnCompleted(continuation);

    public bool IsCompleted { get; private set; }

    public Exception? Error { get; private set; }
    
    public void GetResult()
    {
        if (IsCompleted)
            return;
        _lock ??= SharedObjectPool<ManualResetEventSlim>.Take() ?? new ManualResetEventSlim(false);
        _lock.Wait();

        if (Error != null)
            throw Error;
    }
    
    /// <summary>
    /// Mark this awaiter as complete and invoke the pending action in the current thread.
    /// </summary>
    public void Execute()
    {
        IsCompleted = true;
        _lock?.Set();

        // Error will be throw back to the awaiting thread.
        try
        {
            _continuation?.Invoke();
        }
        catch (Exception error)
        {
            Error = error;
        }
        
        // Recycle lock.
        if (_lock == null)
            return;
        SharedObjectPool<ManualResetEventSlim>.Put(_lock);
        _lock = null;
    }
}