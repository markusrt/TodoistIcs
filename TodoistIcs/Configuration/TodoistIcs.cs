using System.Collections.ObjectModel;
using Todoist.Net.Models;

namespace TodoistIcs.Configuration;

public class TodoistIcs
{
    public string ApiAccessToken { get; set; }
    
    public Uri ICalUrl { get; set; }

    /// <summary>
    /// Events matching one of these strings (Regex supported) are not imported
    /// </summary>
    public Collection<string> IgnoredEvents { get; set; } = new();

    /// <summary>
    /// Priority to assign tasks to, defaults to <see cref="Todoist.Net.Models.Priority.Priority4"/>
    /// </summary>
    public Priority Priority { get; set; } = Priority.Priority4;

    /// <summary>
    /// Number of days to offset Todoist entries from calendar event. E.g. use "-1" for reminders.
    /// </summary>
    public int DayOffset { get; set; }

    /// <summary>
    /// Optional project and section where tasks is to be added to. Usual Todoist syntax "Project /section"
    /// </summary>
    public string Project { get; set; }
}