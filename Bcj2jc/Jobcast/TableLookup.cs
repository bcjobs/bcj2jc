using Dapper;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bcj2jc.Jobcast
{
    class TableLookup
    {
        public TableLookup(IDbConnection connection, string table)
        {
            Connection = connection;
            Table = table;
        }

        IDbConnection Connection { get; }
        string Table { get; }
        ConcurrentDictionary<string, Task<int>> Ids { get; } = new ConcurrentDictionary<string, Task<int>>();

        public async Task<int[]> GetOrAddAsync(IEnumerable<string> names) =>
            await Task.WhenAll(from name in names
                               select GetOrAddAsync(name));

        public async Task<int> GetOrAddAsync(string name)
        {
            var id = await GetAsync(name);
            if (id != 0)
                return id;

            await Connection.ExecuteAsync($"INSERT INTO {Table} (Name) VALUES (@Name)", new { Name = name });
            return await GetAsync(name);
        }

        public async Task<int[]> GetAsync(IEnumerable<string> names) =>
            await Task.WhenAll(from name in names
                               select GetAsync(name));

        public async Task<int> GetAsync(string name) =>
            await Connection.QuerySingleOrDefaultAsync<int>(
                $"SELECT Id FROM {Table} IN @Name",
                new { Name = name });
    }
}
