namespace AsyncToolbox.Test;

public class PendingQueueTest
{
    [Test]
    public void UseAsAsync()
    {
        var queue = new PendingQueue();

        var state = 0;

        Task.Run(async () =>
        {
            state = 1;
            await queue;
            state = 2;
        });

        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process();
        
        Assert.That(state, Is.EqualTo(2));
    }
    
    [Test]
    public void UseAsSync()
    {
        var queue = new PendingQueue();

        var state = 0;
        
        Task.Run(() =>
        {
            state = 1;
            queue.GetAwaiter().GetResult();
            state = 2;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process();
        
        // Wait for sub-task to finish.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(2));
    }

    [Test]
    public void ContextSwitch()
    {
        var queue = new PendingQueue();

        var state = 0;
        
        Task.Run(async () =>
        {
            state = 1;
            await queue;
            state = 2;
            await queue;
            state = 3;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process();
        Assert.That(state, Is.EqualTo(2));
        
        queue.Process();
        Assert.That(state, Is.EqualTo(3));
    }
}