using System.Text.RegularExpressions;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Todoist.Net.Models;

namespace TodoistIcs;

public class CalendarToQuickAddItemService
{
    private readonly Calendar _calendar;
    private readonly Configuration.TodoistIcs _config;

    public CalendarToQuickAddItemService(Calendar calendar, Configuration.TodoistIcs config)
    {
        _calendar = calendar;
        _config = config;
    }

    public IEnumerable<QuickAddItem> CreateQuickAddItems()
    {
        return _calendar.Events.Where(IsNotExcluded).Select(CreateQuickAddItem).ToList();
    }

    private bool IsNotExcluded(CalendarEvent calendarEvent)
    {
        var isExcludedByIgnoreFilter = _config.IgnoredEvents.Any(ignorePattern => Regex.IsMatch(calendarEvent.Summary, ignorePattern));
        var today = DateTime.Now.Date;
        var eventDay = AdjustedEventDate(calendarEvent).Date;
        var isExcludedAsInPast = today > eventDay;
        return !(isExcludedByIgnoreFilter || isExcludedAsInPast);
    }

    private QuickAddItem CreateQuickAddItem(CalendarEvent calendarEvent)
    {
        var quickAddText = $"{calendarEvent.Summary}";
        
        if (calendarEvent.DtStart != null)
        {
            quickAddText += $" {AdjustedEventDate(calendarEvent):yyyy-MM-dd}";
        }

        if (!string.IsNullOrEmpty(_config.Project))
        {
            quickAddText += $" #{_config.Project}";
        }

        quickAddText += $" p{(int)_config.Priority}";
        return new QuickAddItem(quickAddText);
    }

    private IDateTime AdjustedEventDate(CalendarEvent calendarEvent)
    {
        return calendarEvent.DtStart.AddDays(_config.DayOffset);
    }
}