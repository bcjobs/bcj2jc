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
        static void Main(string[] args) => 
            AsyncContext.Run(() => 
                MainAsync(
                    new BCJobsFeed("file:///c:/proj/bcj2jc/Bcj2jc.Tests/bin/Debug/netcoreapp2.1/Feed.xml"),
                    //new BCJobsFeed("https://www.bcjobs.ca/rss/jobs.xml?source=Jobcast&medium=referral&campaign=SearchEngines"),
                    new JobcastDb("Data Source=DIMA-WS;Integrated Security=True;Database=JobcastDev;MultipleActiveResultSets=True")));

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
