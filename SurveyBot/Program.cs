using TelegramUpdater.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegramUpdater(
    (builder) => builder
        .AutoCollectScopedHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
host.Run();