# aspnetcore-audit-middleware

A lightweight library to audit requests / responses in your ASP.Net Core applications.

## How to use it

First of all, add the package to the project

```bash

dotnet add package Com.RFranco.AspNetCore.Audit --version 0.0.1

```

Include the `AuditMiddleware` to the pipeline before `app.UseMvc()`:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    app.UseAuditMiddleware();
    app.UseMvc();
    ...
}
```

Its also possible to configure the middleware overriding the default behavior (ignoring calls that contains`/swagger`, `/health` and `/metrics` in the path) passing a list of path to be ignored as parameters (in the next example, we are excluding calls that contains `/swagger` in the path):

```csharp

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    app.UseAuditMiddleware(new AuditMiddlewareOptions{ExcludedPaths = new List<string>{"/swagger"}});
    app.UseMvc();
    ...
}
```
