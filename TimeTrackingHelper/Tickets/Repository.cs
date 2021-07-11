using System.Threading.Tasks;
using CommandDotNet.Rendering;
using LanguageExt;

namespace TimeTrackingHelper.Tickets
{
    public interface IRepository
    {
        public Task<Option<ITicket>> FindTicket(string identifier);
    }
}