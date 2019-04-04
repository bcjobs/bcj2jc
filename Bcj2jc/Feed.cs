﻿using Bcj2jc.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Bcj2jc
{
    public class Feed : Enumerable<FeedItem>
    {
        public Feed(string url = "https://www.bcjobs.ca/rss/jobs.xml?source=Jobcast&medium=referral&campaign=SearchEngines")
        {
            Url = url;
        }

        string Url { get; }

        public Dictionary<FeedItemId, FeedItem> ToDictionary() =>
            this.ToDictionary(i => i.Id);

        public override IEnumerator<FeedItem> GetEnumerator() =>
            XDocument.Load(Url)
                .XPathSelectElements("//source/job")
                .Select(Parse)
                .Where(i => i != null)
                .GetEnumerator();

        FeedItem Parse(XElement item)
        {
            try
            {
                return new FeedItem(
                    source: item.Element("source").Value,
                    referenceNumber: item.Element("referencenumber").Value,
                    date: DateTime.ParseExact(item.Element("date").Value, "ddd, dd MMM yyyy h:mm:ss tt PDT", null),
                    title: item.Element("title").Value,
                    url: item.Element("url").Value,
                    company: item.Element("company").Value,
                    city: item.Element("city").Value,
                    state: item.Element("state").Value,
                    country: item.Element("country").Value,
                    description: item.Element("description").Value,
                    categories: from c in item.Element("categories").Value.Split(',')
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
