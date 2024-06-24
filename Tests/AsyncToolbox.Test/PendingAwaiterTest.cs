namespace AsyncToolbox.Test;

public class PendingAwaiterTest
{
    [Test]
    public void UseAsAsync()
    {
        var awaiter = new PendingAwaiter();

        var state = 0;
        
        Task.Run(async () =>
        {
            state = 1;
            await awaiter.ToAwaitable();
            state = 2;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
        
        awaiter.Execute();
        
        Assert.That(state, Is.EqualTo(2));
    }
    
    [Test]
    public void UseAsSync()
    {
        var awaiter = new PendingAwaiter();

        var state = 0;
        
        Task.Run(() =>
        {
            state = 1;
            awaiter.GetResult();
            state = 2;
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
        
        awaiter.Execute();
        
        // Wait for sub-task to finish.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(2));
    }
    
    [Test]
    public void PassError()
    {
        var awaiter = new PendingAwaiter();

        var state = 0;
        
        Task.Run(() =>
        {
            state = 1;
            Assert.Throws<Exception>(async ()=> await awaiter.ToAwaitable()); 
            state = 2;
            throw new Exception("Throwing this error for test.");
        });
        
        // Wait for sub-task to register itself.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
        
        awaiter.Execute();
        
        // Wait for sub-task to finish.
        Thread.Sleep(100);
        
        Assert.That(state, Is.EqualTo(1));
    }
}