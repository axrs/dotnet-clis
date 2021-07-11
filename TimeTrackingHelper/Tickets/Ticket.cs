using System;
using System.Threading.Tasks;
using LanguageExt;

namespace TimeTrackingHelper.Tickets
{
    public interface ITicket
    {
        Option<ITicket> Parent();

        Option<ITicket> Epic();

        string Identifier();

        string Summary();

        string Type();

        Option<string> Assignee();

        string Status();

        bool IsEpic();
        
        bool IsSubTask();
    }
}