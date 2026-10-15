# Authorization Sample

This sample shows how to require signed-in users for GroupDocs.Viewer.UI in an ASP.NET Core MVC app. Only users with the `viewer` role can open the viewer and call its API.

## What This Sample Shows

- Cookie authentication with a sign-in page and two demo accounts: `viewer` has the `viewer` role, `guest` has no roles
- An authorization policy applied to both the viewer UI and the viewer API with `RequireAuthorization()`
- The viewer next to the app's own MVC pages and the default route `{controller=Home}/{action=Index}/{id?}`. The default route doesn't reach the viewer API, see [#117](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/issues/117)
- Status codes 401 and 403 instead of redirects to the sign-in page for viewer API requests

## Prerequisites

- .NET 8.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.Authorization
dotnet run
```

Open your browser at the URL shown in the console output, by default `http://localhost:5000`.

## Try It

Open these URLs signed out, as `guest` and as `viewer`. The home page lists them as links.

| URL | Signed out | `guest` | `viewer` |
|---|---|---|---|
| `/viewer/?file=annual-review.docx` | Redirect to the sign-in page | Access denied page | The viewer opens the document |
| `/viewer-api/get-pdf?file=annual-review.docx` | 401 | 403 | The PDF file |
| `/Viewer/GetPdf?file=annual-review.docx` | 404 | 404 | 404 |

## Configuration

### Authentication and the Policy

```csharp
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewerUsers", policy => policy.RequireRole("viewer"));
});
```

### Protecting the Viewer

Apply the policy to both the UI and the API. Applying it to the UI alone leaves the API, and with it every document, open to anyone.

```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroupDocsViewerUI(options =>
{
    options.UIPath = "/viewer";
    options.ApiEndpoint = "/viewer-api";
}).RequireAuthorization("ViewerUsers");

app.MapGroupDocsViewerApi(options =>
{
    options.ApiPath = "/viewer-api";
}).RequireAuthorization("ViewerUsers");

app.MapDefaultControllerRoute();
```

### Status Codes for API Requests

The viewer calls its API with XHR requests, so a redirect to the sign-in page is of no use there. The cookie events answer API requests with 401 and 403 and keep the redirects for the pages:

```csharp
options.Events.OnRedirectToLogin = context =>
    RedirectOrSetStatusCode(context, StatusCodes.Status401Unauthorized);
options.Events.OnRedirectToAccessDenied = context =>
    RedirectOrSetStatusCode(context, StatusCodes.Status403Forbidden);

static Task RedirectOrSetStatusCode(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
{
    if (context.Request.Path.StartsWithSegments("/viewer-api"))
        context.Response.StatusCode = statusCode;
    else
        context.Response.Redirect(context.RedirectUri);

    return Task.CompletedTask;
}
```

## Notes

- **Use the same policy for the UI and the API.** The viewer shows its password prompt for any 403 response from the API. With the same policy, a user who isn't allowed never opens the viewer, so the viewer doesn't receive a 403 from the policy. Keep this in mind if you add your own checks to the API, for example per-document access.
- **Keep the `SameSite` mode of the cookie at `Lax`, the default, or `Strict`.** The viewer API doesn't validate antiforgery tokens, and `SameSite` keeps other sites from sending POST requests, such as file uploads, with the user's cookie.
- **Replace the demo accounts with your identity provider**, for example ASP.NET Core Identity or OpenID Connect. The viewer setup stays the same.

## Project Structure

```
├── Program.cs                     # Authentication, the policy and the viewer setup
├── ViewerAccess.cs                # Policy name, role and viewer paths
├── Controllers/
│   ├── HomeController.cs          # Home page with the documents and the URLs to try
│   └── AccountController.cs       # Demo sign-in, sign-out and access denied
├── Views/                         # Pages of the app
└── Files/                         # Documents to view
```
