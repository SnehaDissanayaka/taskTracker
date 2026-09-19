# Task Tracker — .NET + Angular

A minimal full-stack task tracker, built as a deployment target for the
"Deploying Your First .NET App to Azure" article series. Deliberately small —
the point is to have something real to deploy, not to build a big app.

## Stack
- **Backend**: ASP.NET Core 10 Web API, EF Core with SQL Server (swap to Azure
  SQL when you deploy — that swap is good article content on its own).
- **Frontend**: Angular 17 (standalone components), calling the API over HTTP.

## Prerequisites
- .NET 10 SDK (`dotnet --version` should show 10.x)
- Node.js 18+ and npm
- Angular CLI (`npm install -g @angular/cli`) — optional, `npx ng` also works
- Docker Desktop (for running SQL Server locally)

## Running SQL Server locally

The backend needs a SQL Server instance to connect to. The easiest way is a
Docker container:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<CHOOSE_YOUR_OWN_PASSWORD>" \
  -p 1433:1433 --name tasktracker-sql --hostname tasktracker-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Pick your own password (12+ chars, mixing case/digits/symbols) — don't reuse
one from a public doc or example, and never commit the real value anywhere
in this repo (see [Secrets](#secrets--dont-put-them-in-appsettingsjson)
below). This container only needs to be created once — after that,
`docker start tasktracker-sql` brings it back up. Use
`docker logs tasktracker-sql` to confirm it's ready ("SQL Server is now
ready for client connections").

## Running the backend

The connection string lives in user-secrets, not `appsettings.json` (see
[Secrets](#secrets--dont-put-them-in-appsettingsjson) below) — set it once:

```bash
cd backend/TaskTrackerApi
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TaskTrackerDb;User Id=sa;Password=<THE_PASSWORD_YOU_CHOSE_ABOVE>;TrustServerCertificate=True;"
```

Then run it:

```bash
dotnet restore
dotnet run --launch-profile https
```

`--launch-profile https` matters — the default profile order picks `http`
otherwise, which skips Swagger and HTTPS. This starts the API on
`https://localhost:5001`, auto-creates the `TaskTrackerDb` database and
`Tasks` table on first run (via `EnsureCreated()`), and exposes Swagger UI at
`/swagger` in Development mode. Confirm the port dotnet actually gives you —
if it differs, update `apiBaseUrl` in `src/environments/environment.ts` on
the frontend.

## Running the frontend

```bash
cd frontend/task-tracker-client
npm install
npm start
```

This runs `ng serve` on `http://localhost:4200`. The app calls the API at the
URL set in `src/environments/environment.ts` — update it if your backend is
running on a different port.

## What's here
- `backend/TaskTrackerApi/Controllers/TasksController.cs` — CRUD endpoints (`GET/POST/PUT/DELETE /api/tasks`)
- `backend/TaskTrackerApi/Program.cs` — app startup, CORS policy (currently allows `localhost:4200` only — update this when you deploy the frontend)
- `frontend/task-tracker-client/src/app/app.component.ts` — the whole UI (list, add, complete, delete)

## Setting up git properly

Run this from the **repo root** (`taskTracker/`, the folder containing this
README) — not from inside `backend/`. That matters because the root
`.gitignore` only applies to the folder it lives in and below.

```bash
cd taskTracker
git init
git add .
git commit -m "Initial commit: .NET + Angular task tracker"
git branch -M main
git remote add origin https://github.com/<your-username>/taskTracker.git
git push -u origin main
```

Before you commit, sanity-check what's about to be tracked:

```bash
git status
```

You should **not** see `bin/`, `obj/`, `node_modules/`, `dist/`, `*.db`, or
anything under `postman/` in that list — the root `.gitignore` excludes them.
If any of those show up, it usually means you're running the command from
the wrong folder, or a stray `.gitignore` further down is missing a pattern.

### Secrets — don't put them in appsettings.json

`appsettings.json` keeps `ConnectionStrings:DefaultConnection` empty on
purpose — the real connection string (with the `sa` password) lives in
.NET's user-secrets store instead, set up in the
[Running the backend](#running-the-backend) section above. That stores it
outside the repo entirely (in a user-profile folder, not the project
directory), and `builder.Configuration` picks it up automatically in
Development — no code changes needed. For the actual Azure deployment,
you'll set the connection string as an App Service application setting
instead (covered in the deployment article).

## Next steps (the actual point of this repo)
1. ~~Get it running locally end-to-end.~~ Done — SQL Server via Docker, EF
   Core `EnsureCreated()`, full CRUD verified through the Angular UI.
2. Push to a GitHub repo.
3. Follow the Azure deployment article: create an App Service, set up an
   Azure DevOps pipeline, point the connection string at Azure SQL, and
   deploy.
4. Document what breaks along the way — that's the article.
