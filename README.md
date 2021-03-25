# Users Can Log In Dot Net
A base for all of your basic web app authentication needs written in .NET (C#) with TypeScript and Vue.js. Try it out at [userscanlogin.net](https://userscanlogin.net).

## Background
The name "Users Can Log In" originated from an old joke between me and a classmate. A couple of projects in some of the more introductory
software engineering courses concerning specification and design asked student groups to list functional requirements. Most groups usually
included something along the lines of "users can log in" as one of their first requirements. We found this amusing due to the number of times
we saw groups immediately state this requirement, the terseness of the phrase, and the general implication that any authenticated system could even avoid having some form of user login.

## Intention
I created this project a base for myself to use for any API+single-page-app projects I might want to do in the future, as I sometimes have ideas
for such projects but find myself needing to go through the trouble of setting up some common authentication and authentication-related mechanisms.
I wanted to share it here for anyone else who might find it useful as a boilerplate of sorts.

## Setup
The project is configured to open in Visual Studio 2019 and use IIS Express to host the server in development mode. I have not tested any other IDEs.
The solution contains two projects: UsersCanLogIn.API (the server) and UsersCanLogIn.Web (the client). The "AdminUser" will be created in the database
when the application starts to ensure that an initial user exists in the system for any initial authentication needs. This user will also be the only
user to have a role of Admin, which is essentially just a convenience for any future role creation/use. The "Type" field in "Database" can be set to
InMemory, SQLite, or SQLServer. SQLite and SQLServer connection strings can be set in the "ConnectionStrings" setting. Finally, the "Smtp" configuration
setting is used to set an email account that will be used to send user account verification and password reset emails.

### Configuration Files
Copy UsersCanLogIn.API/appsettings.Example.json and UsersCanLogIn.API/appsettings.Development.Example.json files to files of the same name without
".Example" in the name. These files are in the .gitignore as they will serve as the actual application configuration settings, which should not be
committed. Set the fields appropriately in these newly created files based on the examples. 

### Server Development (ASP.NET Core Web API Project)
In VS, install the nuget dependencies, and set the startup project to UsersCanLogIn.API. The project is already configured to launch in IIS Express
for development. Upon launching, it will open the Swagger documentation for viewing and testing API endpoints. The code and project structure assume
familiarity with development of web-based APIs, so further details are beyond the scope of this document.

### Client Development (Vue.js Single Page App Project)
To run the client, first navigate to the UsersCanLogIn.Web directory and run `npm install` to install dependencies. Then, run the webpack development
server by running `npm run serve`. Visit localhost:8080 to work with the front end in development mode.

### Client production build
The server is also configured to statically host the Vue.js production build bundle from the web root (wwwroot directory). To create a production build,
run `npm run build` from UsersCanLogIn.Web (this can also be done in VS by building the UsersCanLogIn.Web project). The production build will automatically
be placed in the wwwroot directory in UsersCanLogIn.API. All non-API routes on the server will automatically fall back to the production Vue.js app build.
