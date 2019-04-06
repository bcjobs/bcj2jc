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
            await Connection.QueryAsync<long>($"SELECT ReferenceId FROM Koopla.Jobs WHERE Source = @Source", new { Source = source });

        public async Task RemoveAsync(string source, long id) =>
            await Connection.ExecuteAsync(
                "UPDATE Koopla.Jobs SET StatusId = 4, DeletedDate = GetUtcDate() WHERE Source = @Source AND ReferenceId = @ReferenceId", new { Source = source, ReferenceId = $"{id}" });
        
        public async Task InsertAsync(Job item)
        {
            var companyId = await CompanyAsync(item.Source, item.Company);
            var provinceId = await ProvinceAsync(item.State);
            var countryId = await CountryAsync(item.Country);

            await Connection.ExecuteAsync(
                "INSERT INTO Koopla.Jobs (ReferenceId, Source, StatusId, CreatedDate, ModifiedDate, PublishDate, Name, Description, CompanyId, City, " +
                "ProvinceId, CountryId, ApplicantRoutingTypeId, DescriptionFormat, ApplicationUrl) " +
                "VALUES (@ReferenceId, @Source, 1, GetUtcDate(), GetUtcDate(), @PublishDate, @Name, @Description, @CompanyId, @City, @ProvinceId, @CountryId, 2, 'html', @ApplicationUrl)",
                new
                {
                    ReferenceId = $"{item.Id}",
                    Source = item.Source,
                    PublishDate = item.Date,                    
                    Name = item.Title,
                    Description = item.Description,
                    CompanyId = companyId,
                    City = item.City,
                    ProvinceId = provinceId,
                    CountryId = countryId,
                    ApplicationUrl = item.Url
                });

            var id = await Connection.ExecuteScalarAsync<int>(
                "SELECT Id FROM Koopla.Jobs WHERE ReferenceId = @ReferenceId AND Source = @Source",
                new { ReferenceId = item.Id, Source = item.Source });

            foreach (var categoryId in await CategoryAsync(companyId, item.Categories))
                await Connection.ExecuteAsync(
                    "INSERT INTO Koopla.Jobs_JobCategories (JobId, JobCategoryId) VALUES (@JobId, @JobCategoryId)",
                    new { JobId = id, JobCategoryId = categoryId });        
        }

        public async Task UpdateAsync(Job item)
        {
            var companyId = await CompanyAsync(item.Source, item.Company);
            var provinceId = await ProvinceAsync(item.State);
            var countryId = await CountryAsync(item.Country);

            await Connection.ExecuteAsync(
                "UPDATE Koopla.Jobs SET " +
                "ModifiedDate = GetUtcDate(), StatusId = 1, DeletedDate=NULL, PublishDate = @PublishDate, Name = @Name, " + 
                "Description = @Description, CompanyId = @CompanyId, City = @City, ProvinceId = @ProvinceId, CountryId = @CountryId " +
                "WHERE Source = @Source AND ReferenceId = @ReferenceId",
                new
                {
                    ReferenceId = $"{item.Id}",
                    Source = item.Source,
                    PublishDate = item.Date,
                    Name = item.Title,
                    Description = item.Description,
                    CompanyId = companyId,
                    City = item.City,
                    ProvinceId = provinceId,
                    CountryId = countryId
                });

            var id = await Connection.ExecuteScalarAsync<int>(
                "SELECT Id FROM Koopla.Jobs WHERE ReferenceId = @ReferenceId AND Source = @Source",
                new { ReferenceId = item.Id, Source = item.Source });

            await Connection.ExecuteAsync("DELETE FROM Koopla.Jobs_JobCategories WHERE JobId = @JobId", new { JobId = id });
            foreach (var categoryId in await CategoryAsync(companyId, item.Categories))
                await Connection.ExecuteAsync(
                    "INSERT INTO Koopla.Jobs_JobCategories (JobId, JobCategoryId) VALUES (@JobId, @JobCategoryId)",
                    new { JobId = id, JobCategoryId = categoryId });
        }

        async Task<int> CompanyAsync(string source, string name) =>
            await Companies.GetOrAddAsync(
                name,
                "INSERT INTO Koopla.Companies " +
                "(Source, Name, CreatedDate, ModifiedDate, CompanyStatusId, SearchEnginesEnabled, LCID, TaxExempt, ShowPageFooter, RequiresResume, UnlimitedJobOverride, RouteSharesToApplicationUrl) " +
                $"VALUES ('{source}', @Name, GetUtcDate(), GetUtcDate(), 1, 0, 9, 0, 1, 1, 1, 0)",
                "SELECT Id FROM Koopla.Companies " +
                $"WHERE Name = @Name AND Source = '{source}'");

        async Task<int?> ProvinceAsync(string name) =>
            await Provinces.GetOrNullAsync(
                name,
                "SELECT p.Id FROM Koopla.Provinces AS p " +
                "INNER JOIN Koopla.Countries AS c ON c.Id = p.CountryId " +
                "WHERE p.AlphaCode = @Name AND c.Name = 'Canada'");

        async Task<int?> CountryAsync(string name) =>
            await Countries.GetOrNullAsync(
                name,
                "SELECT Id FROM Koopla.Countries " +
                "WHERE Alpha2_ISO3166_1 = @Name");

        async Task<int[]> CategoryAsync(int companyId, IEnumerable<string> names) =>
            await Categories.GetOrAddAsync(
                names,
                $"INSERT INTO Koopla.JobCategories (CompanyId, Name) VALUES ({companyId}, @Name)",
                $"SELECT Id FROM Koopla.JobCategories WHERE Name = @Name AND (CompanyId = {companyId} OR CompanyId IS NULL)");
    }
}
