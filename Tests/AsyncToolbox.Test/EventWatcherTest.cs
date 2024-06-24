namespace AsyncToolbox.Test;

public class EventWatcherTest
{
    public class SampleObjectWithEvent
    {
        public event Action? OnEvent;

        public void TriggerEvent()
        {
            OnEvent?.Invoke();
        }
    }
    
    [Test]
    public void WaitForOnce()
    {
        var state = 0;

        var eventSource = new SampleObjectWithEvent();

        Task.Run(async () =>
        {
            var watcher = new EventWatcher();
            eventSource.OnEvent += watcher.EventHandler;

            await watcher;

            state = 1;
        });
        
        // Wait sub-task to register the watcher.
        Thread.Sleep(100);
        
        eventSource.TriggerEvent();
        
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
    }
    
    [Test]
    public void WaitForMultipleTimes()
    {
        var state = 0;

        var eventSource = new SampleObjectWithEvent();

        Task.Run(async () =>
        {
            var watcher = new EventWatcher();
            eventSource.OnEvent += watcher.EventHandler;

            await watcher;

            state = 1;
            
            await watcher;

            state = 2;
        });
        
        // Wait sub-task to register the watcher.
        Thread.Sleep(100);
        
        eventSource.TriggerEvent();
        
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
    }

}