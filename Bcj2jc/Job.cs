using System;
using System.Collections.Generic;
using System.Linq;

namespace Bcj2jc
{
    public class Job
    {
        public Job(long id, string source, string referenceNumber, 
            string title, DateTime date, string url, string company, 
            string city, string state, string country, string description,
            IEnumerable<string> categories)
        {
            Id = id;
            Source = source;
            ReferenceNumber = referenceNumber;
            Title = title;
            Date = date;
            Url = url;
            Company = company;
            City = city;
            State = state;
            Country = country;
            Description = description;
            Categories = categories.ToArray();
        }

        public long Id { get; }
        public string Source { get; }
        public string ReferenceNumber { get; }
        public string Title { get; }
        public DateTime Date { get; }
        public string Url { get; }
        public string Company { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string Description { get; }
        public IReadOnlyList<string> Categories { get; }
        public bool DaysOlder(int days) => Date.AddDays(days) < DateTime.Now; 

        public override string ToString() => Title;
    }
}
