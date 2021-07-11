using System;

namespace TimeTrackingHelper.TimesEntry
{
    public class TimesEntry
    {
        public string Developer { get; init; }
        public DateTime Date { get; init; }
        public string Ticket { get; init; }
        public string Activity { get; init; }
        public int Minutes { get; init; }
        public string Notes { get; init; }
    }
}