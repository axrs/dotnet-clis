using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using CommandDotNet;
using CommandDotNet.Rendering;
using ExcelDataReader;
using LanguageExt;
using static LanguageExt.List;
using static System.Text.Encoding;

namespace TimeTrackingHelper.TimesEntry
{
    static class ExcelUtils
    {
        /// <summary>
        /// True if the Excel Sheet (tab) is a Monthly Tech Team Times Entry Tab
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns>True if the Excel Sheet is a Monthly Tech Team Times Tab</returns>
        public static bool IsTimeSheetTab(DataTable sheet)
        {
            return Prelude.parseDateTime(sheet.TableName).IsSome;
        }

        /// <summary>
        /// True if the Data Row (excel spreadsheet row), is a valid times Entry
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static bool ValidTimeEntry(DataRow dataRow)
        {
            return Prelude.parseDateTime(dataRow[1].ToString()).IsSome;
        }

        /// <summary>
        /// Converts the Data Row into a Times Entry
        /// </summary>
        /// <param name="console"></param>
        /// <param name="dataRow">Excel Times Entry Data Row</param>
        /// <returns>Times Entry if it's valid. Null otherwise</returns>
        public static TimesEntry ToTimeEntry(IConsole console, DataRow dataRow)
        {
            try
            {
                return new TimesEntry()
                {
                    Developer = dataRow[0].ToString(),
                    Date = DateTime.Parse(dataRow[1].ToString()!),
                    Ticket = dataRow[3].ToString()?.ToUpper(),
                    Activity = dataRow[4].ToString()?.ToUpper(),
                    Minutes = (int) Math.Floor(double.Parse(dataRow[5].ToString() ?? "0")),
                    Notes = dataRow[6].ToString()
                };
            }
            catch (FormatException)
            {
                console.WriteLine("Skipping Entry due to parsing errors. Likely invalid time format:");
                console.WriteLine($"\t{dataRow[0]},{dataRow[1]},{dataRow[3]},{dataRow[4]},{dataRow[5]},{dataRow[6]}");
            }

            return null;
        }

        public static readonly Func<IConsole, DataRow, TimesEntry> toTimeEntry = ToTimeEntry;

        public static List<TimesEntry> LoadTimeSheets(IConsole console, DataTableCollection sheets)
        {
            List<TimesEntry> times = new();
            return filter(sheets.Cast<DataTable>(), IsTimeSheetTab)
                .Fold(times, (times1, sheet) =>
                {
                    var timesEntries = sheet.Rows.Cast<DataRow>()
                        .Filter(ValidTimeEntry)
                        .Map(Prelude.par(toTimeEntry, console))
                        .Filter(option => option != null)
                        .ToList();
                    return append(times1, timesEntries).ToList();
                });
        }

        public static List<TimesEntry> ReadTimeEntries(IConsole console, FileInfo file)
        {
            var fileFullName = file.FullName;
            console.WriteLine($"Reading Time Entries from: {fileFullName}");
            RegisterProvider(CodePagesEncodingProvider.Instance);
            using var stream = File.Open(fileFullName, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var workbookTabs = reader.AsDataSet().Tables;
            return LoadTimeSheets(console, workbookTabs);
        }
    }
}