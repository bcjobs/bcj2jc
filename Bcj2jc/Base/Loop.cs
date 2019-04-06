using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bcj2jc.Jobcast.Base
{
    public static class Loop
    {
        public static async Task<TOut[]> ToArray<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Task<TOut>> action)
        {
            var result = new List<TOut>();
            foreach (var item in source)
                result.Add(await action(item));

            return result.ToArray();
        }
    }
}
