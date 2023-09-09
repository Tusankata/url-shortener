# URL-Shortener

(Inspired by [Milan Jovanovic's](https://github.com/m-jovanovic) [video](https://www.youtube.com/watch?v=SLpUKAGnm-g))

A small API project that shortens URLs. Written mainly for practicing purposes.

## Technologies and Patterns

The architecture is based on this [project](https://github.com/nadirbad/VerticalSliceArchitecture/).

- [ASP.NET API with .NET 7](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0)
- CQRS with [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- Minimal API endpoints with [Carter](https://github.com/CarterCommunity/Carter)
- [Entity Framework Core 7](https://docs.microsoft.com/en-us/ef/core/)
- [Serilog](https://github.com/serilog/serilog)
- [.NET Test Container](https://github.com/testcontainers/testcontainers-dotnet), [xUnit](https://xunit.net/), [FluentAssertions](https://github.com/fluentassertions/fluentassertions)

## Projects

The solution has 2 projects.
### Api

The entry point of the application. All controllers are moved to the **Application** project.

### Application

Contains all the application logic. Services, Entities and other common concerns are here. The business logic is in the `Features` folder, each feature file contains the endpoint, the command/query handlers, validators and the response/request models.

## Getting Started

1. Install the latest [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)

### Docker

To run the application inside a docker container, you will need to set-up the database connection string in the `docker-compose.yml` file.

I am using environment variables to set my User ID and Password, if you choose to do the same, then you'll need to create a `.env` file in the root of the project and add the following lines:

```
# .env
MSSQL_USER_ID=<YOUR USERNAME>
MSSQL_PASSWORD=<YOUR PASSWORD>
```

Otherwise you can just replace the environment variables in the `docker-compose.yml` file with your username and password.

After that, you will need to create the database:
`dotnet ef database update -p .\src\UrlShortener.Application\ -s .\src\UrlShortener.Api\`

Then you can run the application:
`docker-compose up --build -d`
(or `docker-compose up --build` if you want to see the logs)
*Note: The first time you run the application, it will take a while to download the images and build the application.*

### Run Locally

To run the application locally, you will need to set-up the database connection string in the `appsettings.json` file.

After that, you will need to create the database:
`dotnet ef database update -p .\src\UrlShortener.Application\ -s .\src\UrlShortener.Api\`

Then you can run the application:
`dotnet run -p .\src\UrlShortener.Api\`

### Testing

Currently, my integration tests are ran against a docker test container. To run the tests, simply run:
`dotnet test .\tests\UrlShortener.Application.Integration.Tests\`
(or `dotnet test .\tests\UrlShortener.Application.Integration.Tests\ --logger "console;verbosity=detailed"` if you want to see the logs)
*Note: You can add `--no-build` to skip building the solution before running the tests.*

**IMPORTANT**: You will need to have Docker Desktop installed and running for the tests to run.

## Thoughts

A lot of the technologies used in this project are new to me, so I am still learning how to use them properly. I am also trying to follow the best practices and patterns as much as I can, but I am sure there are a lot of things that can be improved.

There's quite a few things I'd like to improve in the future:
- Add more tests
- Add more logging
- Add proper validation
- Better error handling
- Figure out why a log file isn't being created

The list goes on, but I think this is a good start.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
