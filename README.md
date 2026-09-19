# Task Tracker — .NET + Angular

A minimal full-stack task tracker, built as a deployment target for the
"Deploying Your First .NET App to Azure" article series. Deliberately small —
the point is to have something real to deploy, not to build a big app.

## Stack
- **Backend**: ASP.NET Core 8 Web API, EF Core with SQLite (swap to Azure SQL
  or PostgreSQL when you deploy — that swap is good article content on its own).
- **Frontend**: Angular 17 (standalone components), calling the API over HTTP.

## Prerequisites
- .NET 8 SDK (`dotnet --version` should show 8.x)
- Node.js 18+ and npm
- Angular CLI (`npm install -g @angular/cli`) — optional, `npx ng` also works

## Running the backend

```bash
cd backend/TaskTrackerApi
dotnet restore
dotnet run
```

This starts the API (default: `https://localhost:5001`), auto-creates
`tasks.db` (SQLite) on first run, and exposes Swagger UI at `/swagger` in
Development mode. Confirm the port dotnet actually gives you — if it differs,
update `apiBaseUrl` in `src/environments/environment.ts` on the frontend.

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

The SQLite connection string in `appsettings.json` is harmless (it's just a
local file path), but once you deploy and swap to Azure SQL or PostgreSQL,
that connection string will contain real credentials. Don't commit those.
Use .NET's built-in secret manager for local dev instead:

```bash
cd backend/TaskTrackerApi
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your real connection string>"
```

This stores it outside the repo entirely (in a user-profile folder, not the
project directory), and `builder.Configuration` picks it up automatically in
Development — no code changes needed. For the actual Azure deployment,
you'll set the connection string as an App Service application setting
instead (covered in the deployment article).

## Next steps (the actual point of this repo)
1. Get it running locally end-to-end.
2. Push to a GitHub repo.
3. Follow the Azure deployment article: create an App Service, set up an
   Azure DevOps pipeline, swap SQLite for Azure SQL, and deploy.
4. Document what breaks along the way — that's the article.
