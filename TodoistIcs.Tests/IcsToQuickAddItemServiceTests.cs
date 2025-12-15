using System.Collections.ObjectModel;
using FluentAssertions;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NUnit.Framework;
using Todoist.Net.Models;

namespace TodoistIcs.Tests
{
    public class IcsToQuickAddItemServiceTests
    {
        [Test]
        public void CreateSut_DoesNotThrow()
        {
            Action creatingSut = () => CreateSut(out _, out _);

            creatingSut.Should().NotThrow();
        }

        [Test]
        public void EmptyCalendar_DoesCreateEmptyQuickAddItems()
        {
            var sut = CreateSut(out _, out _);

            sut.CreateQuickAddItems().Should().BeEmpty();
        }

        [TestCase(1)]
        [TestCase(20)]
        [TestCase(0)]
        [TestCase(149)]
        public void CalendarWithEvents_CreatesQuickAddItems(int amount)
        {
            var sut = CreateSut(out var calendar, out _);
            for (var i = 0; i < amount; i++)
            {
                calendar.Events.Add(CreateEvent("test"));
            }

            sut.CreateQuickAddItems().Should().HaveCount(amount);
        }

        [Test]
        public void IgnoredEvents_AreNotAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.IgnoredEvents = new Collection<string> { "Ignore.*", "Not need.*", "Blacklist" };
            calendar.Events.Add(CreateEvent("Summary"));
            calendar.Events.Add(CreateEvent("Ignore"));
            calendar.Events.Add(CreateEvent("Ignore Me"));
            calendar.Events.Add(CreateEvent("Not needed"));
            calendar.Events.Add(CreateEvent("Not needed at all"));
            calendar.Events.Add(CreateEvent("Blacklist"));

            sut.CreateQuickAddItems().Should().HaveCount(1);
        }

        [Test]
        public void EmptyIgnoredEvents_AllAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.IgnoredEvents = new Collection<string>();
            calendar.Events.Add(CreateEvent("Summary"));
            calendar.Events.Add(CreateEvent(""));
            calendar.Events.Add(CreateEvent("Remember Me"));
            calendar.Events.Add(CreateEvent("Important Task"));
            calendar.Events.Add(CreateEvent("Try this"));
            calendar.Events.Add(CreateEvent("Watch out"));

            sut.CreateQuickAddItems().Should().HaveCount(6);
        }


        [Test]
        public void PastEvents_AreNotAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.DayOffset = -2;
            calendar.Events.Add(CreateEvent("Reminder two days ago", DateTime.UtcNow));
            calendar.Events.Add(CreateEvent("Reminder one days ago", DateTime.UtcNow.AddDays(1)));
            calendar.Events.Add(CreateEvent("Reminder today", DateTime.UtcNow.AddDays(2)));
            calendar.Events.Add(CreateEvent("Reminder tomorrow", DateTime.UtcNow.AddDays(3)));

            sut.CreateQuickAddItems().Should().OnlyContain(c => c.Text.Contains("today") || c.Text.Contains("tomorrow"));
        }

        [Test]
        public void CalendarEventSummary_IsAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out _);
            calendar.Events.Add(CreateEvent("Summary"));

            var item = sut.CreateQuickAddItems().Single();

            item.Text.Should().Be("Summary 2222-11-19 p4");
        }

        [Test]
        public void CalendarEventDate_IsAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out _);
            calendar.Events.Add(CreateEvent("Birthday"));

            var item = sut.CreateQuickAddItems().Single();

            item.Text.Should().Be("Birthday 2222-11-19 p4");
        }

        [Test]
        public void ConfiguredPriority_IsAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.Priority = Priority.Priority2;
            calendar.Events.Add(CreateEvent("Birthday"));

            var item = sut.CreateQuickAddItems().Single();

            item.Text.Should().Be("Birthday 2222-11-19 p2");
        }

        [Test]
        public void ConfiguredProject_IsAddedToQuickAddItem()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.Project = "Family /chores";
            calendar.Events.Add(CreateEvent("Birthday"));

            var item = sut.CreateQuickAddItems().Single();

            item.Text.Should().Be("Birthday 2222-11-19 #Family /chores p4");
        }

        [Test]
        public void CalendarEventDate_IsAddedToQuickAddItemWithOffset()
        {
            var sut = CreateSut(out var calendar, out var config);
            config.DayOffset = -1;
            calendar.Events.Add(CreateEvent("Birthday", new DateTime(8022, 11, 10)));

            var item = sut.CreateQuickAddItems().Single();

            item.Text.Should().Be("Birthday 8022-11-09 p4");
        }
        
        private static CalendarEvent CreateEvent(string summary, DateTime? start = null)
        {
            var dtStart = start == null ? new CalDateTime(new DateTime(2222, 11, 19)) : new CalDateTime(start.Value);
            return new CalendarEvent() { Summary = summary, DtStart = dtStart};
        }

        private static CalendarToQuickAddItemService CreateSut(out Calendar calendar, out Configuration.TodoistIcs config)
        {
            calendar = new Calendar();
            config = new Configuration.TodoistIcs();
            return new CalendarToQuickAddItemService(calendar, config);
        }
    }
}