using Microsoft.Extensions.FileProviders;
using WebTemplate.Services.Implementations;
using WebTemplate.Services.Interfaces;
try
{
   



    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CORS", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("http://localhost:5501",
                               "https://localhost:5501",
                               "http://127.0.0.1:5501",
                               "https://127.0.0.1:5501");
        });
    });

    builder.Services.AddSingleton<FileSystemWatcher>(provider =>
    {
        var watcher = new FileSystemWatcher();
        // Konfigurišite watcher ako je potrebno
        return watcher;
    });

    //var portArgument = Array.Find(args, arg => arg.StartsWith("--port="));
    //if (portArgument != null)
    //{
    //    var portValue = portArgument.Split('=')[1];
    //    if (int.TryParse(portValue, out var port))
    //    {
    //        builder.Configuration["Application:ListeningPort"] = port.ToString();
    //    }
    //}

    builder.Services.AddTransient<IAlgorithm, Railfence>(); //dodavanje Railfence algoritma
    builder.Services.AddTransient<IFactory, Factory>(); //dodavanje fabrike koja kreira algoritme

    builder.Services.AddControllers();//dodavanje kontrolera

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    //Dodavanje file System Watcher-a

    string targetDirectory = "C:\\Users\\matej\\Desktop\\Zastita informacija\\Projekat\\TransferCryptoFileApp\\Target";
    string outputDirectory = "C:\\Users\\matej\\Desktop\\Zastita informacija\\Projekat\\TransferCryptoFileApp\\X";
    var logger = app.Services.GetRequiredService<ILogger<FileSystemWatcherService>>();
    var factory = app.Services.GetRequiredService<IFactory>();
    var fswService = new FileSystemWatcherService(targetDirectory, outputDirectory, logger,factory);
    fswService.StartWatching();
    app.Lifetime.ApplicationStopping.Register(() => fswService.StopWatching());

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("CORS");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
    throw;
}
