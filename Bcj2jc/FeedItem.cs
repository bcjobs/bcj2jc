using System;

namespace Bcj2jc
{
    public class FeedItem
    {
        public FeedItem(string source, string referenceNumber, 
            string title, DateTime date, string url, string company, 
            string city, string state, string country, string description)
        {
            Id = new FeedItemId(source, referenceNumber);
            Title = title;
            Date = date;
            Url = url;
            Company = company;
            City = city;
            State = state;
            Country = country;
            Description = description;
        }

        public FeedItemId Id { get; }
        public string Title { get; }
        public DateTime Date { get; }
        public string Url { get; }
        public string Company { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string Description { get; }

        public override string ToString() => Title;
    }
}
