using TelegramUpdater.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegramUpdater(
    (builder) => builder
        .CollectHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
host.Run();
