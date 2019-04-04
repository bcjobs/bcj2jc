using System;
using System.Collections.Generic;
using System.Text;

namespace Bcj2jc
{
    interface IJobFeed : IEnumerable<Job>
    {
        string Source { get; }
    }
}
