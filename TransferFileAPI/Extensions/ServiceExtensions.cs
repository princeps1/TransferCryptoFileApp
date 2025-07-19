namespace WebTemplate.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddCors(options =>
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

        services.AddSingleton<FSWService>();


        services.AddTransient<IAlgorithm, Railfence>(); //dodavanje Railfence algoritma
        services.AddTransient<Railfence>(); //dodavanje Railfence algoritma
        services.AddTransient<XXTEA>(); //dodavanje XXTEA algoritma
        services.AddTransient<XXTEACBC>(); //dodavanje XXTEA algoritma
        services.AddTransient<IFactory, Factory>(); //dodavanje fabrike koja kreira algoritme

        services.AddControllers();//dodavanje kontrolera


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
