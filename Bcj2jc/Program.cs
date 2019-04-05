using Bcj2jc.BCJobs;
using Bcj2jc.Jobcast;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace Bcj2jc
{
    class Program
    {
        // bcj2jc "feed-url" "sql-conn-str"
        static void Main(string[] args) => 
            AsyncContext.Run(() => 
                MainAsync(
                    new BCJobsFeed(args[0]),
                    new JobcastDb(args[1])));

        static async Task MainAsync(IJobFeed feed, IJobDb db)
        {
            var dbIds = new HashSet<long>(await db.IdsAsync(feed.Source));
            var jobs = feed.ToArray();
            foreach (var job in jobs)
                if (!dbIds.Contains(job.Id))
                {
                    Write("+");
                    await db.InsertAsync(job);
                }
                else if (!job.DaysOlder(7))
                {
                    Write("*");
                    await db.UpdateAsync(job);
                }

            var jobIds = new HashSet<long>(from j in jobs select j.Id);
            foreach (var id in dbIds)
                if (!jobIds.Contains(id))
                {
                    Write("-");
                    await db.RemoveAsync(feed.Source, id);
                }
        }
    }
}
