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

    builder.Services.AddSingleton<FileSystemWatcherService>();


    builder.Services.AddTransient<IAlgorithm, Railfence>(); //dodavanje Railfence algoritma
    builder.Services.AddTransient<Railfence>(); //dodavanje Railfence algoritma
    builder.Services.AddTransient<XXTEA>(); //dodavanje XXTEA algoritma
    builder.Services.AddTransient<XXTEACBC>(); //dodavanje XXTEA algoritma
    builder.Services.AddTransient<IFactory, Factory>(); //dodavanje fabrike koja kreira algoritme

    builder.Services.AddControllers();//dodavanje kontrolera

   
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    /////////////
    /////////////
    /////////////
    var app = builder.Build();

    var logger = app.Services.GetRequiredService<ILogger<FileSystemWatcherService>>();
    var factory = app.Services.GetRequiredService<IFactory>();
    var watcherService = app.Services.GetRequiredService<FileSystemWatcherService>();
    watcherService.StartWatching();
    app.Lifetime.ApplicationStopping.Register(() => watcherService.StopWatching());

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
