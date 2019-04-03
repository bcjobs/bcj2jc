using System.Collections.Generic;
using Bcj2jc.Base;

namespace Bcj2jc
{
    public class FeedItemId : ValueObject<FeedItemId>
    {
        public FeedItemId(string source, string referenceNumber)
        {
            Source = source;
            ReferenceNumber = referenceNumber;
        }

        public string Source { get; }
        public string ReferenceNumber { get; }

        protected override IEnumerable<object> EqualityCheckAttributes =>
            new object[] { Source, ReferenceNumber };

        public override string ToString() => $"{ReferenceNumber}@{Source}";
    }
}
