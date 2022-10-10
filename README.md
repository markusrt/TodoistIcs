# Todoist ICS importer utility

Tiny utility to import ICS files hosted online to your Todoist tasks.
Currently it only adds reminders for future events. I.e. events from the
past are excluded via filtering.

## Build and code quality

[![CodeQL](https://github.com/markusrt/TodoistIcs/actions/workflows/codeql-analysis.yml/badge.svg?branch=master)](https://github.com/markusrt/TodoistIcs/actions/workflows/codeql-analysis.yml)

## How to use the tool

- Install [Dotnet SDK](https://dotnet.microsoft.com/en-us/download)
- Register a new Todoist app using their [app console](https://developer.todoist.com/appconsole.html) 
- Generate test token
- Put test token to `appsettings.json`
- Configure other appsettings, see [configuration model](TodoistIcs/Configuration/TodoistIcs.cs) for config options
- Run tool using `dotnet run --project TodoistIcs/TodoistIcs.csproj`

## Potential next steps

This "tool" is currently provided as-is and not very userfriendly. However it does the job ;)

In case I one day find a time to fix this the following features could be handy:

- Check why project sections are not working via API quickadditem (`#Project /Section` works via client app)
- Create release pipeline to at least produce an executeable
- Support import of past events (e.g. as yearly  recurring reminders)
- Use proper OAuth flow with Todoist API
- Create fancy web UI
