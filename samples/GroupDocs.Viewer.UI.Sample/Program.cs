using GroupDocs.Viewer.UI.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
        builder.WithOrigins("http://localhost:4200") // Allow specific origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.PreloadPages = 3; // Number of pages to create on first request
        config.DefaultLanguage = LanguageCode.French;
        config.SupportedLanguages = [LanguageCode.French, LanguageCode.English, LanguageCode.Italian];
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Viewer UI can be accessed at '/viewer' endpoint.");
        });
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });
await app.RunAsync();