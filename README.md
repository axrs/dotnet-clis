# dotnet-CLIs

A Collection of DotNet CLI tools

## kanban-summary (source) (start_date) (end_date)

> Runs a Kanban Summary from a time tracking spreadsheet
 
    Environment Variables:
    - <JIRA_USER>    : JIRA Username
    - <JIRA_API_KEY> : JIRA API Key

~~~bash
echo " $test_case in $file"
dotnet run \
    --project ./TimeTrackingHelper -- \
    kanban-summary \
    --start-date "$start_date" \
    --end-date "$end_date" \
    --source "$source"
~~~