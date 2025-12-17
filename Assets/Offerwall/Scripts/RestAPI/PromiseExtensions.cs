using System;
using System.Threading.Tasks;
using RSG;

namespace Rest.API
{
    public static class PromiseExtensions
    {
        public static Task<T> ToTask<T>(this IPromise<T> promise)
        {
            var tcs = new TaskCompletionSource<T>();
        
            promise
                .Then(result =>
                {
                    tcs.SetResult(result);
                })
                .Catch(ex => tcs.SetException(ex));

            return tcs.Task;
        }

        public static Task ToTask(this IPromise promise)
        {
            var tcs = new TaskCompletionSource<object>();

            promise
                .Then(() => tcs.SetResult(null))
                .Catch(ex => tcs.SetException(ex));

            return tcs.Task;
        }
    }
}