using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
var webRootProvider = new PhysicalFileProvider(webRootPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = webRootProvider
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = webRootProvider
});

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.Run();
