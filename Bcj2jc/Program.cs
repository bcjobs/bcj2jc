using Bcj2jc.BCJobs;
using Bcj2jc.Jobcast;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bcj2jc
{
    class Program
    {
        static void Main(string[] args) => 
            AsyncContext.Run(() => 
                MainAsync(
                    new BCJobsFeed("https://www.bcjobs.ca/rss/jobs.xml?source=Jobcast&medium=referral&campaign=SearchEngines"),
                    new JobcastDb("Data Source=DIMA-WS;Integrated Security=True;Database=JobcastDev")));

        static async Task MainAsync(IJobFeed feed, IJobDb db)
        {
            var dbIds = new HashSet<long>(await db.IdsAsync(feed.Source));
            var jobs = feed.ToArray();
            foreach (var job in jobs)
                if (!dbIds.Contains(job.Id))
                    await db.InsertAsync(job);
                else if (!job.DaysOlder(7))
                    await db.UpdateAsync(job);                      

            var jobIds = new HashSet<long>(from j in jobs select j.Id);
            foreach (var id in dbIds)
                if (!jobIds.Contains(id))
                    await db.RemoveAsync(id);
        }
    }
}
