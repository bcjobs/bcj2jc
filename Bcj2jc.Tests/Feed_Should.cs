using Bcj2jc.BCJobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using static System.IO.Path;
using static System.Net.WebUtility;

namespace Bcj2jc.Tests
{
    [TestClass]
    public class Feed_Should
    {
        [TestMethod]
        public void Parse()
        {
            var feed = new BCJobsFeed(FeedPath)
                .ToDictionary(j => j.Id);

            var id = 4263L;
            var item = feed[id];

            Assert.AreEqual(id, item.Id);
            Assert.AreEqual("BCJobs", item.Source);
            Assert.AreEqual("04190-4263", item.ReferenceNumber);
            Assert.AreEqual("Driver - Clothing Pickups | Casual", item.Title);
            Assert.AreEqual("BC", item.State);
            Assert.AreEqual("CA", item.Country);
            Assert.AreEqual(new DateTime(2019, 4, 2, 11, 23, 18), item.Date);
            Assert.AreEqual("Developmental Disabilities Association (DDA)", item.Company);
            Assert.AreEqual("Richmond", item.City);
            Assert.AreEqual("https://www.bcjobs.ca/jobs/driver-clothing-pickups-casual-richmond-04190-4263?utm_source=Jobcast&utm_medium=referral&utm_campaign=SearchEngines", item.Url);
            CollectionAssert.AreEqual(new[] { "Transportation and Warehousing", "Non-profit", "Labour" }, item.Categories.ToArray());
        }

        string FeedPath => new Uri(
            Combine(GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Feed.xml"))
            .AbsoluteUri;
    }
}
