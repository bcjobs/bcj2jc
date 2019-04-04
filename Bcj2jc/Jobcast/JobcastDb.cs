using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcj2jc.Jobcast
{
    public class JobcastDb : IJobDb
    {
        public JobcastDb(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Companies = new TableLookup(Connection, "Koopla.Companies");
            Categories = new TableLookup(Connection, "Koopla.JobCategories");
            Provinces = new TableLookup(Connection, "Koopla.Provinces");
            Countries = new TableLookup(Connection, "Koopla.Countries");
        }

        IDbConnection Connection { get; }
        TableLookup Companies { get; }
        TableLookup Categories { get; }
        TableLookup Provinces { get; }
        TableLookup Countries { get; }

        public async Task<IEnumerable<long>> IdsAsync(string source) =>
            await Connection.QueryAsync<long>("SELECT ReferenceId FROM Koopla.Jobs");

        public async Task InsertAsync(Job item)
        {
            await Connection.ExecuteAsync(
                "INSERT INTO Koopla.Jobs (ReferenceId, PublishDate, Name, Description, Company, City, Province, Country) " + 
                "VALUES (@ReferenceId, @PublishDate, @Name, @Description, @Company, @City, @Province, @Country)",
                new
                {
                    ReferenceId = item.Id,
                    PublishDate = item.Date,
                    Name = item.Title,
                    Description = item.Description,
                    Company = await Companies.GetOrAddAsync(item.Company),
                    City = item.City,
                    Province = await Provinces.GetAsync(item.State),
                    Country = await Countries.GetAsync(item.Country)
                });

            var id = await Connection.ExecuteScalarAsync<int>(
                "SELECT Id FROM Koopla.Jobs WHERE ReferenceId = @ReferenceId",
                new { ReferenceId = item.Id });

            await Task.WhenAll(
                from category in await Categories.GetOrAddAsync(item.Categories)
                select Connection.ExecuteAsync(
                    "INSERT INTO Koopla.Jobs_JobCategories (JobId, JobCategoryId) VALUES (@JobId, @JobCategoryId)",
                    new { JobId = id, JobCategoryId = category }));
        }

        public async Task UpdateAsync(Job item)
        {
            

        }

        public async Task RemoveAsync(long id)
        {
            
        }        
    }
}
