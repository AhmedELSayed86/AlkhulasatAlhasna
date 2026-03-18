namespace Alkhulasat.DataAccess.Context
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Run(taskFactory))
        { }
    }
}
