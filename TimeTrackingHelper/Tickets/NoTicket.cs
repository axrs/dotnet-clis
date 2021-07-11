using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Option<TimeTrackingHelper.Tickets.ITicket>;

namespace TimeTrackingHelper.Tickets
{
    public class NoTicket : ITicket
    {
        private readonly string _summary = "";
        private readonly string _assignee = "";

        public NoTicket()
        {
        }
        
        public NoTicket(string summary, string assignee)
        {
            _summary = summary;
            _assignee = assignee;
        }

        public Option<ITicket> Parent()
        {
            return None;
        }

        public Option<ITicket> Epic()
        {
            return None;
        }

        public string Identifier()
        {
            return "";
        }

        public string Summary()
        {
            return _summary;
        }

        public string Type()
        {
            return "";
        }

        public Option<string> Assignee()
        {
            return _assignee;
        }

        public string Status()
        {
            return "";
        }

        public bool IsEpic()
        {
            return false;
        }

        public bool IsSubTask()
        {
            return false;
        }
    }
}