namespace AsyncToolbox;

public readonly struct AwaiterWrapper<TAwaiter>(TAwaiter awaiter)
{
    public TAwaiter GetAwaiter() => awaiter;
}

public static class AwaiterWrapperExtension
{
    public static AwaiterWrapper<TAwaiter> ToAwaitable<TAwaiter>(this TAwaiter awaiter)
        => new(awaiter);
}