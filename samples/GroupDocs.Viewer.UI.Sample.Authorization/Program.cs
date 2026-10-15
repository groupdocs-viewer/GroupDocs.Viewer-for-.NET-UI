using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Sample.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // The viewer calls its API with XHR requests, a redirect to the sign-in page is of no use there
        options.Events.OnRedirectToLogin = context =>
            RedirectOrSetStatusCode(context, StatusCodes.Status401Unauthorized);
        options.Events.OnRedirectToAccessDenied = context =>
            RedirectOrSetStatusCode(context, StatusCodes.Status403Forbidden);
    });

builder.Services.AddAuthorization(options =>
{
    // Only users with the "viewer" role can open the viewer and call its API
    options.AddPolicy(ViewerAccess.Policy, policy => policy.RequireRole(ViewerAccess.Role));
});

var viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();
    });

builder.Services
    .AddControllersWithViews()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Apply the policy to both the UI and the API, the API alone gives access to every document
app.MapGroupDocsViewerUI(options =>
{
    options.UIPath = ViewerAccess.UIPath;
    options.ApiEndpoint = ViewerAccess.ApiPath;
}).RequireAuthorization(ViewerAccess.Policy);

app.MapGroupDocsViewerApi(options =>
{
    options.ApiPath = ViewerAccess.ApiPath;
}).RequireAuthorization(ViewerAccess.Policy);

// The pages of this app. The default route doesn't reach the viewer API, e.g. /Viewer/GetPdf returns 404,
// see https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/issues/117
app.MapDefaultControllerRoute();

await app.RunAsync();

static Task RedirectOrSetStatusCode(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
{
    if (context.Request.Path.StartsWithSegments(ViewerAccess.ApiPath))
        context.Response.StatusCode = statusCode;
    else
        context.Response.Redirect(context.RedirectUri);

    return Task.CompletedTask;
}
