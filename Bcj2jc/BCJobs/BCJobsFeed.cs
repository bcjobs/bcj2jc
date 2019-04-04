using Bcj2jc.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Bcj2jc.BCJobs
{
    public class BCJobsFeed : Enumerable<Job>, IJobFeed
    {
        public BCJobsFeed(string url)
        {
            Url = url;
        }

        string Url { get; }
        public string Source => "BCJobs";
                
        public override IEnumerator<Job> GetEnumerator() =>
            XDocument.Load(Url)
                .XPathSelectElements("//source/job")
                .Select(Parse)
                .Where(i => i != null)
                .GetEnumerator();

        Job Parse(XElement item)
        {
            try
            {
                return new Job(
                    id: long.Parse(item.Element("referencenumber").Value.Split('-').Last()),
                    source: Source,
                    referenceNumber: item.Element("referencenumber").Value,
                    date: DateTime.ParseExact(item.Element("date").Value, "ddd, dd MMM yyyy h:mm:ss tt PDT", null),
                    title: item.Element("title").Value,
                    url: item.Element("url").Value,
                    company: item.Element("company").Value,
                    city: item.Element("city").Value,
                    state: item.Element("state").Value,
                    country: item.Element("country").Value,
                    description: item.Element("description").Value,
                    categories: from c in item.Element("category").Value.Split(',')
                                select c.Trim());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                return null;
            }
        }
    }
}
