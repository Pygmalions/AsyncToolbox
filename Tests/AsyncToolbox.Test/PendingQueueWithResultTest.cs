namespace AsyncToolbox.Test;

public class PendingQueueWithResultTest
{
    [Test]
    public void UseAsAsync()
    {
        var queue = new PendingQueue<int>();

        var state = 0;

        Task.Run(async () =>
        {
            state = 1;
            
            var result = await queue;
            Assert.That(result, Is.EqualTo(-1));
            
            state = 2;
        });

        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process(-1);
        
        Assert.That(state, Is.EqualTo(2));
    }
    
    [Test]
    public void UseAsSync()
    {
        var queue = new PendingQueue<int>();

        var state = 0;
        
        Task.Run(() =>
        {
            state = 1;
            var result = queue.GetAwaiter().GetResult();
            Assert.That(result, Is.EqualTo(-1));
            state = 2;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process(-1);
        
        // Wait for sub-task to finish.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(2));
    }

    [Test]
    public void ContextSwitch()
    {
        var queue = new PendingQueue<int>();

        var state = 0;
        
        Task.Run(async () =>
        {
            state = 1;
            var result1 = await queue;
            Assert.That(result1, Is.EqualTo(-1));
            state = 2;
            var result2 = await queue;
            Assert.That(result2, Is.EqualTo(-2));
            state = 3;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        queue.Process(-1);
        Assert.That(state, Is.EqualTo(2));
        
        queue.Process(-2);
        
        Assert.That(state, Is.EqualTo(3));
    }
}