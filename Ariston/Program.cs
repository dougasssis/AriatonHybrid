using Ariston.Services;
using Ariston.Thermos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using Wolverine;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseWolverine((context, options) =>
    {
        options.Services.AddSingleton<Thermo>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
    });

builder.ApplyOaktonExtensions();

IHost app = builder.Build();
return await app.RunOaktonCommands(args);