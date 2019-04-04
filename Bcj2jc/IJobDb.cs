using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bcj2jc
{
    public interface IJobDb
    {
        Task<HashSet<long>> IdsAsync(string source);
        Task InsertAsync(Job item);
        Task UpdateAsync(Job item);
        Task RemoveAsync(long id);
    }
}
