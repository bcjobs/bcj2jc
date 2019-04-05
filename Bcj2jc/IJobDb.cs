using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bcj2jc
{
    public interface IJobDb
    {
        Task<IEnumerable<long>> IdsAsync(string source);
        Task RemoveAsync(string source, long id);
        Task InsertAsync(Job item);
        Task UpdateAsync(Job item);
    }
}
