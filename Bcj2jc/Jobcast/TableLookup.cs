using Bcj2jc.Jobcast.Base;
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

        public async Task<int[]> GetOrAddAsync(IEnumerable<string> names, string insert = null, string select = null) =>
            await names.ToArray(name => GetOrAddAsync(name, insert, select));

        public async Task<int> GetOrAddAsync(string name, string insert = null, string select = null) =>
            await Ids.GetOrAdd(name, async n =>
            {
                var id = await Connection.QuerySingleOrDefaultAsync<int>(
                    select ?? $"SELECT Id FROM {Table} WHERE Name = @Name",
                    new { Name = name });

                if (id != 0)
                    return id;

                await Connection.ExecuteAsync(insert ?? $"INSERT INTO {Table} (Name) VALUES (@Name)", new { Name = name });
                return await Connection.QuerySingleOrDefaultAsync<int>(
                    select ?? $"SELECT Id FROM {Table} WHERE Name = @Name",
                    new { Name = name });
            });

        public async Task<int[]> GetAsync(IEnumerable<string> names, string select = null) =>
            await names.ToArray(name => GetAsync(name, select));

        public async Task<int> GetAsync(string name, string select = null) =>
            await Ids.GetOrAdd(name, async n =>
                await Connection.QuerySingleOrDefaultAsync<int>(
                    select ?? $"SELECT Id FROM {Table} WHERE Name = @Name",
                    new { Name = name }));

        public async Task<int?> GetOrNullAsync(string name, string select = null) =>
            await GetAsync(name, select) == 0 
            ? (int?)null 
            : await GetAsync(name, select);
    }
}
