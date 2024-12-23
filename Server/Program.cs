using Microsoft.Extensions.FileProviders;
try
{
   

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CORS", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("http://localhost:5500",
                               "https://localhost:5500",
                               "http://127.0.0.1:5500",
                               "https://127.0.0.1:5500");
        });
    });




    builder.Services.AddControllers();//dodavanje kontrolera

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    //Dodavanje file System Watcher-a

    string targetDirectory = "C:\\Users\\matej\\Desktop\\Zastita informacija\\Projekat\\CryptoFileApp\\Target";
    string outputDirectory = "C:\\Users\\matej\\Desktop\\Zastita informacija\\Projekat\\CryptoFileApp\\X";
    var fswService = new FileSystemWatcherService(targetDirectory, outputDirectory, app.Services.GetRequiredService<ILogger<FileSystemWatcherService>>());
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
