using System.Runtime.CompilerServices;

namespace AsyncToolbox;

public interface IAwaiter : ICriticalNotifyCompletion
{
    bool IsCompleted { get; }

    void GetResult();
}

public interface IAwaiter<out TResult> : ICriticalNotifyCompletion
{
    bool IsCompleted { get; }
    
    TResult GetResult();
}