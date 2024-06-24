using AsyncToolbox.Utilities;

namespace AsyncToolbox;

public class PendingAwaiter<TResult>(Func<Action, Action?>? wrapper = null) : IAwaiter<TResult>
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
    
    public TResult Result { get; private set; } = default!;
    
    public TResult GetResult()
    {
        if (IsCompleted)
            return Result;
        
        _lock ??= SharedObjectPool<ManualResetEventSlim>.Take() ?? new ManualResetEventSlim(false);
        while (!IsCompleted)
            _lock.Wait();

        if (Error != null)
            throw Error;
        
        return Result;
    }

    /// <summary>
    /// Mark this awaiter as complete and invoke the pending action with the specific result in the current thread.
    /// </summary>
    public void Execute(TResult result)
    {
        IsCompleted = true;
        Result = result;
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