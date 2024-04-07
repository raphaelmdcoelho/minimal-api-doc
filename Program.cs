var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps("certification/mycert.pfx", "123456");
    });
    // Production approach:
    // serverOptions.ListenAnyIP(443, listenOptions =>
    // {
    //     listenOptions.UseHttps("/container/certs/mycert.pfx", "certificatePassword");
    // });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
