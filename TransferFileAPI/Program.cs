var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<FSWService>>();
var factory = app.Services.GetRequiredService<IFactory>();
var watcherService = app.Services.GetRequiredService<FSWService>();

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
