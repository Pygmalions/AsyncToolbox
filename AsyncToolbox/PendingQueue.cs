using System.Collections.Concurrent;
using AsyncToolbox.Utilities;

namespace AsyncToolbox;

public class PendingQueue : IAwaitable
{
    private ConcurrentBag<PendingAwaiter> _context = new();
    
    /// <summary>
    /// Process all pending actions with a custom handler.
    /// </summary>
    /// <param name="handler">
    /// Handler to enumerate and execute pending actions. 
    /// </param>
    public void Process(Action<ConcurrentBag<PendingAwaiter>> handler)
    {
        // Replace current context with new context for incoming pending actions.
        var context = _context;
        _context = SharedObjectPool<ConcurrentBag<PendingAwaiter>>.Take() ?? new ConcurrentBag<PendingAwaiter>();
        
        // Handle actions.
        handler(context);
        
        // Recycle the context.
        context.Clear();
        SharedObjectPool<ConcurrentBag<PendingAwaiter>>.Put(context);
    }
    
    /// <summary>
    /// Invoke all pending actions one by one in the current thread.
    /// </summary>
    public void Process()
    {
        // Replace current context with new context for incoming pending actions.
        var context = _context;
        _context = SharedObjectPool<ConcurrentBag<PendingAwaiter>>.Take() ?? new ConcurrentBag<PendingAwaiter>();
        
        // Handle actions.
        foreach (var awaiter in context)
            awaiter.Execute();
        
        // Recycle the context.
        context.Clear();
        SharedObjectPool<ConcurrentBag<PendingAwaiter>>.Put(context);
    }
    
    /// <summary>
    /// Get an awaiter with a wrapper.
    /// </summary>
    /// <param name="wrapper">
    /// Wrap the registered continuation action into another action.
    /// </param>
    /// <returns>Awaiter with the specific wrapper.</returns>
    public PendingAwaiter GetAwaiter(Func<Action, Action?> wrapper)
    {
        var awaiter = new PendingAwaiter(wrapper);
        _context.Add(awaiter);
        return awaiter;
    }
    
    public IAwaiter GetAwaiter()
    {
        var awaiter = new PendingAwaiter();
        _context.Add(awaiter);
        return awaiter;
    }
}