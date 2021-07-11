using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Rendering;
using CsvHelper;
using LanguageExt;
using TimeTrackingHelper.Tickets;
using TimeTrackingHelper.TimesEntry;
using static TimeTrackingHelper.TimesEntry.Util;
using static LanguageExt.Prelude;

namespace TimeTrackingHelper
{
    public class CommandLineInterface
    {
        [Command(Name = "kanban-summary",
            Usage = "kanban-summary " +
                    "--start-date <yyyy-mm-dd> " +
                    "--end-date <yyyy-mm-dd> " +
                    "--source <tech-team-activities.xlsx>",
            Description = "Produces a Kanban summary report for the specified date range")]
        public async Task KanbanSummary(
            IConsole console,
            [Option(LongName = "start-date", Description = "First day of Kanban cycle to summarise (inclusive)")]
            DateTime startDate,
            [Option(LongName = "end-date", Description = "Last day of Kanban cycle to summarise (exclusive)")]
            DateTime endDate,
            [Option(LongName = "source", Description = "Tech Team Activities File")]
            FileInfo source,
            [EnvVar("JIRA_USER")] [Option(LongName = "jira-user", Description = "JIRA Username")]
            string jiraUserName,
            [EnvVar("JIRA_API_KEY")] [Option(LongName = "jira-key", Description = "JIRA API Key")]
            string jiraApiKey)
        {
            var repository = new JiraRepository(console, jiraUserName, jiraApiKey);
            console.WriteLine($"Running Kanban Summary for {startDate} to {endDate}...");
            var entriesToDate = ExcelUtils.ReadTimeEntries(console, source);
            console.WriteLine($"{entriesToDate.Count} Records Loaded...");

            console.WriteLine("Aggregating to Date...");
            var sumPerTicketToDate = SumPerTicket(entriesToDate);

            console.WriteLine("Aggregating times in date range...");
            var entriesInRange = entriesToDate
                .Filter(par(isBetween, startDate, endDate))
                .ToList();
            var ticketEntries = entriesInRange.Filter(HasTicketIdentifier).ToList();
            var otherEntries = entriesInRange.Filter(HasNoTicketIdentifier).ToList();
            var sumPerTicketInRange = SumPerTicket(ticketEntries);

            console.WriteLine($"Summarised {sumPerTicketInRange.Count} tickets amassing to " +
                              $"{sumPerTicketInRange.Sum()} total minutes...");
            var ticketSummaries = sumPerTicketInRange
                .Map((identifier, timeSpent) =>
                {
                    var ticket = repository
                        .FindTicket(identifier)
                        .Result
                        .IfNone(new NoTicket());
                    return KanbanSummaryCsvDto.ValueOf(ticket, timeSpent);
                }).Values
                .ToList();
            var otherSummaries = otherEntries
                .Map(e =>
                {
                    var ticket = new NoTicket(e.Notes, e.Developer);
                    return KanbanSummaryCsvDto.ValueOf(ticket, e.Minutes);
                })
                .ToList();
            await WriteToCsv(startDate, endDate, ticketSummaries, otherSummaries);
        }

        private static async Task WriteToCsv(DateTime startDate, DateTime endDate, List<KanbanSummaryCsvDto> ticketSummaries, List<KanbanSummaryCsvDto> otherSummaries)
        {
            var summaries = ticketSummaries
                .Append(otherSummaries)
                .OrderBy(v => v.EpicId)
                .ThenBy(v => v.ParentId)
                .ThenBy(v => v.Id)
                .ThenBy(v => v.Minutes);
            await using var writer = new StreamWriter($"kanban-summary-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(summaries);
        }
    }
}