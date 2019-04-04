﻿using Bcj2jc.BCJobs;
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
                    new JobcastDb("TBD")));

        static async Task MainAsync(IJobFeed feed, IJobDb db)
        {
            var dbIds = await db.IdsAsync(feed.Source);
            var jobs = feed.ToArray();
            foreach (var job in jobs)
                if (!job.DaysOlder(7))
                    if (dbIds.Contains(job.Id))
                        await db.UpdateAsync(job);
                    else
                        await db.InsertAsync(job);

            var jobIds = new HashSet<long>(from j in jobs select j.Id);
            foreach (var id in dbIds)
                if (!jobIds.Contains(id))
                    await db.RemoveAsync(id);
        }
    }
}
