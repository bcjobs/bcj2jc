using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bcj2jc.Jobcast
{
    public class JobcastDb : IJobDb
    {
        public JobcastDb(string connectionString)
        {
            ConnectionString = connectionString;
        }

        string ConnectionString { get; }

        public Task<HashSet<long>> IdsAsync(string source)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Job item)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Job item)
        {
            throw new NotImplementedException();
        }
    }
}
