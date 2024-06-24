using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AsyncToolbox;

/// <summary>
/// When the watched event is triggered, the <see cref="AwaitingTask"/> task will be completed.
/// Though this watcher is still applicable when an event will be triggered for multiple times,
/// <see cref="PendingQueue"/> would have better performance at that case.
/// </summary>
public class EventWatcher
{
    private TaskCompletionSource _taskSource = new();
    
    public readonly Action EventHandler;
    
    public EventWatcher()
    {
        EventHandler = Notify;
    }
    
    private void Notify()
    {
        var source = _taskSource;
        _taskSource = new TaskCompletionSource();
        source.SetResult();
    }
    
    /// <summary>
    /// This task will return when the bound event is triggered.
    /// </summary>
    public Task AwaitingTask => _taskSource.Task;
    
    public TaskAwaiter GetAwaiter() => _taskSource.Task.GetAwaiter();
}