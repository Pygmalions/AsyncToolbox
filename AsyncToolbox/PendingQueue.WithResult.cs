using System.Collections.Concurrent;
using AsyncToolbox.Utilities;

namespace AsyncToolbox;

public class PendingQueue<TResult> : IAwaitable<TResult>
{
    private ConcurrentBag<PendingAwaiter<TResult>> _context = new();
    
    /// <summary>
    /// Process all pending actions with a custom handler.
    /// </summary>
    /// <param name="handler">
    /// Handler to enumerate and execute pending actions. 
    /// </param>
    public void Process(Action<ConcurrentBag<PendingAwaiter<TResult>>> handler)
    {
        // Replace current context with new context for incoming pending actions.
        var context = _context;
        _context = SharedObjectPool<ConcurrentBag<PendingAwaiter<TResult>>>.Take() ?? 
                    new ConcurrentBag<PendingAwaiter<TResult>>();
        
        // Handle actions.
        handler(context);
        
        // Recycle the context.
        context.Clear();
        SharedObjectPool<ConcurrentBag<PendingAwaiter<TResult>>>.Put(context);
    }

    /// <summary>
    /// Invoke all pending actions one by one with the specific result in the current thread.
    /// </summary>
    /// <param name="result">Result to pass to pending actions.</param>
    public void Process(TResult result)
    {
        // Replace current context with new context for incoming pending actions.
        var context = _context;
        _context = SharedObjectPool<ConcurrentBag<PendingAwaiter<TResult>>>.Take() ?? 
                    new ConcurrentBag<PendingAwaiter<TResult>>();
        
        // Handle actions.
        foreach (var awaiter in context)
            awaiter.Execute(result);
        
        // Recycle the context.
        context.Clear();
        SharedObjectPool<ConcurrentBag<PendingAwaiter<TResult>>>.Put(context);
    }

    /// <summary>
    /// Get an awaiter with a wrapper.
    /// </summary>
    /// <param name="wrapper">
    /// Wrap the registered continuation action into another action.
    /// </param>
    /// <returns>Awaiter with the specific wrapper.</returns>
    public PendingAwaiter<TResult> GetAwaiter(Func<Action, Action?> wrapper)
    {
        var awaiter = new PendingAwaiter<TResult>(wrapper);
        _context.Add(awaiter);
        return awaiter;
    }
    
    public IAwaiter<TResult> GetAwaiter()
    {
        var awaiter = new PendingAwaiter<TResult>();
        _context.Add(awaiter);
        return awaiter;
    }
}