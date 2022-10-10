using System.Net;
using Ical.Net;
using Microsoft.Extensions.Configuration;
using Todoist.Net;
using Todoist.Net.Models;
using TodoistIcs;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var appSettings = config.GetRequiredSection("TodoistIcs").Get<TodoistIcs.Configuration.TodoistIcs>();

var calendarContent = await new HttpClient().GetStringAsync(appSettings.ICalUrl);
var service = new CalendarToQuickAddItemService(Calendar.Load(calendarContent), appSettings);

var items = service.CreateQuickAddItems();
if(!items.Any()) 
{
    System.Console.WriteLine("Calendar file did not contain any future events");
}
else
{
    var client = new TodoistClient(appSettings.ApiAccessToken);
    foreach (var item in items)
    {
        Console.WriteLine($"Adding task: {item.Text}");
        await client.Items.QuickAddAsync(item);
    }

    Console.WriteLine($"{items.Count()} task(s) added");
}