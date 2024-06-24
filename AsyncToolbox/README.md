# AsyncToolbox

This library provides utilities for asynchronous programming.

## Interfaces for Duck Types in async/await Mechanism

### IAwaitable

Classes implements `IAwaitable` interface can be 'awaited',
which means you can use `await` keyword before their instances.

### IAwaiter

Compiler will automatically generate state machine for async methods,
and in these methods, code after `await` keyword will be packed into a delegate,
and then passed into the `OnCompleted(Action)` method of the awaiter returned from 
the `GetAwaiter()` function of the awaitable object.

## Utilities

### PendingAwaiter

`PendingAwaiter` allows you to control when and where the remaining code after `await` keyword
is executed.

By invoke the `Execute()` function of `PendingAwaiter`, the remaining async code will be executed
in the current thread.

There is also a generic version `PendingAwaiter<TResult>`, and you can use it to pass result value to
the remaining async code.

### PendingQueue

`PendingQueue` allows you deeply customize how pending actions to be executed.
It is useful when you want these pending actions to be executed one by one,
or you do not want wot instantiate too many `TaskCompletionSource`.

Await a `PendingQueue` will add the remaining code into the queue.

You can use `Process()` method to execute pending actions one by one,
or use `Process(Action<ConcurrentBag<PendingAwaiter>>)` to customize how these pending actions are executed.

There is also a generic version `PendingQueue<TResult>`, and you can use it to pass result value to
the remaining async code.

### AwaitableWrapper

This is used to wrap an awaiter into an awaitable object,
then you can use `await` keyword on that awaiter.

An extension method `ToAwaitable()` is provided for `IAwaiter` interface.

### EventWatcher

This allows you to wait for an event to happen in an asyc method.

```csharp
async void YourAsyncMethod()
{
    var watcher = new EventWatcher();
    
    // Register the event watcher to the event.
    traditionalEvent += watcher.EventHandler;
    
    // ...
    
    // This code will continue after the watched event is triggered.
    await watcher;
    
    // ...
    
    // Remove the handler from the event to stop watching.
    // Alert: if you don't do so, the event watcher may not be released.
    traditionalEvent -= watcher.EventHandler;
}
```