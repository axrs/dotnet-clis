using System;
using System.Collections.Generic;
using LanguageExt;
using static System.String;

namespace TimeTrackingHelper.TimesEntry
{
    public static class Util
    {
        /// <summary>
        /// Returns a sum of each task by ticket
        /// </summary>
        /// <param name="timesEntries"></param>
        /// <returns></returns>
        public static Map<string, int> SumPerTicket(List<TimesEntry> timesEntries)
        {
            return timesEntries
                .Filter(HasTicketIdentifier)
                .Fold(Map<string, int>.Empty, (m, t) =>
                    m.AddOrUpdate(t.Ticket, i => i + t.Minutes, t.Minutes));
        }

        /// <summary>
        /// True if the Times Entry is between the specified date range
        /// </summary>
        /// <param name="startDate">Start of Date Range (Inclusive)</param>
        /// <param name="endDate">End of Date Range (Exclusive)</param>
        /// <param name="timesEntry"></param>
        /// <returns></returns>
        public static bool IsBetween(DateTime startDate, DateTime endDate, TimesEntry timesEntry)
        {
            return startDate <= timesEntry.Date && timesEntry.Date < endDate;
        }

        // ReSharper disable once InconsistentNaming
        public static readonly Func<DateTime, DateTime, TimesEntry, bool> isBetween = IsBetween;

        /// <summary>
        /// True if the TimesEntry has a Ticket identifier
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool HasTicketIdentifier(TimesEntry entry)
        {
            return !IsNullOrEmpty(entry.Ticket);
        }

        // ReSharper disable once InconsistentNaming
        public static readonly Func<TimesEntry, bool> hasTicketIdentifier = HasTicketIdentifier;
        
        public static bool HasNoTicketIdentifier(TimesEntry entry)
        {
            return !HasTicketIdentifier(entry);
        }
    }
}