using System;
using System.Collections.Generic;
using System.Text;

namespace InsightsLibrary.Model
{
    public struct TimeRange
    {
        public DateTime? start;
        public DateTime? end;

        public TimeRange(DateTime? start, DateTime? end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
