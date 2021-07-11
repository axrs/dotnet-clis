using TimeTrackingHelper.Tickets;

namespace TimeTrackingHelper
{
    public class KanbanSummaryCsvDto
    {
        public static KanbanSummaryCsvDto ValueOf(ITicket ticket, int minutes)
        {
            string epicId = "";
            string epicSummary = "";
            string parentId = "";
            string parentSummary = "";
            string id = "";
            string summary = "";

            if (ticket.IsEpic())
            {
                epicId = ticket.Identifier();
                epicSummary = ticket.Summary();
            }
            else if (ticket.IsSubTask())
            {
                id = ticket.Identifier();
                summary = ticket.Summary();
                ticket.Parent().IfSome(p =>
                {
                    parentId = p.Identifier();
                    parentSummary = p.Summary();
                });
                ticket.Epic().IfSomeAsync(e =>
                {
                    epicId = e.Identifier();
                    epicSummary = e.Summary();
                });
            }
            else
            {
                parentId = ticket.Identifier();
                parentSummary = ticket.Summary();
                ticket.Epic().IfSome(e =>
                {
                    epicId = e.Identifier();
                    epicSummary = e.Summary();
                });
            }

            return new KanbanSummaryCsvDto
            {
                EpicId = epicId,
                EpicSummary = epicSummary,
                ParentId = parentId,
                ParentSummary = parentSummary,
                Id = id,
                Type = ticket.Type(),
                Summary = summary,
                Assignee = ticket.Assignee().IfNone(""),
                Status = ticket.Status(),
                Minutes = minutes
            };
        }

        public string EpicId { get; set; }
        public string EpicSummary { get; set; }

        public string ParentId { get; set; }
        public string ParentSummary { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }

        public string Summary { get; set; }
        public string Assignee { get; set; }

        public string Status { get; set; }

        public int Minutes { get; set; }
    }
}