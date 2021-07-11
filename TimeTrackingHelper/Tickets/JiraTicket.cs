using System;
using LanguageExt;
using LanguageExt.ClassInstances;
using Newtonsoft.Json.Linq;

namespace TimeTrackingHelper.Tickets
{
    public class JiraTicket : ITicket
    {
        private readonly IRepository _repository;
        private readonly JObject _jiraResponse;
        private readonly string _json;

        public JiraTicket(IRepository repository, string jiraJsonResponse)
        {
            _repository = repository;
            _json = jiraJsonResponse;
            _jiraResponse = JObject.Parse(jiraJsonResponse);
        }

        private string SelectToken(string key)
        {
            return _jiraResponse.SelectToken(key).Value<string>();
        }

        public Option<ITicket> Parent()
        {
            var parent = _jiraResponse.SelectToken("$.fields.parent");
            if (parent is not {HasValues: true}) return Option<ITicket>.None;
            var parentId = SelectToken("$.fields.parent.key");
            return _repository.FindTicket(parentId).Result;
        }

        public Option<ITicket> Epic()
        {
            if (IsSubTask())
            {
                return Parent().Match(Some: parent => parent.Epic(),
                    None: Option<ITicket>.None);
            }
            var epicField = _jiraResponse.SelectToken("$.fields.customfield_10004");
            if (epicField.IsNull()) return Option<ITicket>.None;
            var epicId = epicField.Value<String>();
            return string.IsNullOrEmpty(epicId) 
                ? Option<ITicket>.None 
                : _repository.FindTicket(epicId).Result;
        }

        public string Identifier()
        {
            return SelectToken("$.key");
        }

        public string Summary()
        {
            return SelectToken("$.fields.summary");
        }

        public string Type()
        {
            return SelectToken("$.fields.issuetype.name");
        }

        public Option<string> Assignee()
        {
            var assignee = _jiraResponse.SelectToken("$.fields.assignee");
            return assignee.HasValues
                ? assignee.SelectToken(".displayName").Value<string>()
                : Option<string>.None;
        }

        public string Status()
        {
            return SelectToken("$.fields.status.name");
        }

        public bool IsEpic()
        {
            return Type() == "Epic";
        }

        public bool IsSubTask()
        {
            return Type() == "Sub-task";
        }
    }
}