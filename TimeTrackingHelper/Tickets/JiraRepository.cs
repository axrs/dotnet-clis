using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Rendering;
using Flurl;
using Flurl.Http;
using LanguageExt;

namespace TimeTrackingHelper.Tickets
{
    public class JiraRepository : IRepository
    {
        private readonly IConsole _console;
        private readonly string _userName;
        private readonly string _apiKey;
        private readonly Map<string, JiraTicket> _cache;

        public JiraRepository(IConsole console, string userName, string apiKey)
        {
            _console = console;
            _userName = userName;
            _apiKey = apiKey;
            _cache = Map<string, JiraTicket>.Empty;
        }

        public async Task<Option<ITicket>> FindTicket(string identifier)
        {
            if (_cache.ContainsKey(identifier)) return _cache[identifier];
            _console.WriteLine($"Getting {identifier} from JIRA");
            try
            {
                var url = "https://jesims.atlassian.net/rest/api/3/issue/"
                    .AppendPathSegment(identifier)
                    .SetQueryParams(new
                    {
                        fields = "issuetype,assignee,status,summary,parent,customfield_10004"
                    })
                    .WithBasicAuth(_userName, _apiKey)
                    .WithHeader("accept", "application/json");
                var json = await url.GetStringAsync();
                var ticket = new JiraTicket(this, json);
                _cache.TryAdd(identifier, ticket);
                return ticket;
            }
            catch (FlurlHttpException ex)
            {
                _console.Write($"Error returned from {ex.Call.Request.Url}: {ex.Message}");
            }

            return Option<ITicket>.None;
        }
    }
}