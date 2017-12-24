# Diff Web API

ASP .Net Web Api used to compare base64 encoded content.

- Providing 2 http endpoints that accepts JSON base64 encoded binary data on both endpoints;
- Providing a endpoint for diff comparison:
  - Should the data between both JSON be the same, then a JSON with a Message property is returned saying so.
  - Should the data between both JSON have different sizes, then a JSON with a Message property is returned saying so.
  - Should the data between both JSON have the same size, but different content, then a JSON with the following properties is returned:
    - Message
	- Length
	- DiffOffsets

## Prerequisites

* Visual Studio 2015 or 2017 (C# 6 compiler required)
* MSSQL Server (Express should be enough)
  
## Built with

* [Visual Studio 2017] (https://www.visualstudio.com/) - IDE for .NET projects. For this project, also used to host localdb and Web Api.
* [Entity Framework](https://docs.microsoft.com/en-us/ef/) - ORM
* [ASP .NET Web Api 2] (https://msdn.microsoft.com/en-us/library/dn448365(v=vs.118).aspx) - Web Api framework.
* [Moq] (https://github.com/moq/moq4) - Mocking framework for unit tests.
* [FluentAssertions](http://fluentassertions.com/) - Assertion framework for unit tests.
* [MSSQL Server] (https://www.microsoft.com/en-us/sql-server/sql-server-2016) - Data Base Management System to store/maintain data.
* [Ninject] (http://www.ninject.org/) - Dependency injection container.
* [Swagger] (https://swagger.io/) - API tooling - Provides a clean an easy interfacte to interact with the hosted web api.

## Installation

*No (auto)deployment package is in place yet. Kept it simple though for the purpose of reading, compiling and running the code.

# Via Visual Studio (2015 or 2017) using IIS Express
1) Set WAES.Cris.WebApi as startup project.
2) Run the solution
3) WAES.Cris.WebApi is listening on http://localhost:56216/ (hosted on VS IIS Express) - You can change the port at your will.
4) The DbContext is set up to create the database (WAES) should it not exist. The database will be created on (LocalDb)\MSSQLLocalDB.
The default db_owner credentials comes from the very person running the VS.
5) Swagger will be avaliable @ http://localhost:56216/swagger

# Via IIS
1) Extract WAES.Cris.WebApi.zip to a folder of your choice.
2) You can create a new web site listening on any port, or add an app under the Default Web Site.
3) Create an app pool with .NET CLR 4.0 and integrated CLR. All the rest can be the default.
4) Connect to your local SQL Server instance (it can be a simple SQL Express) and create a fresh database named WAES. In order to simplify authentication & delegation, please, set up a SQL account and have it added as db_owner to this new database.
5) Change the very WaesDbContext connectionstring to point to your local database instance, and add both userid and password attributes.
6) Kick off the API URL/Swagger (Ex: http://localhost/WAES.Cris/Swagger) and enjoy.

## Running the tests
Inside VS, navigate to 'Test' menu -> Run -> All Tests.

## Rest API

All methods are described in the swagger endpoint. It should be http://localhost:56216/swagger in case you're running the api via VS.

## Improvements

- Come up with an automated/simple deployment process. Docker, WebDeploy package or something.

## Author

Cristiano Alves da Silva (https://www.linkedin.com/in/cristiano-alves-da-silva/)
