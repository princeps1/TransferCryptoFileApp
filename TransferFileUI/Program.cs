using Refit;
using TransferFileUI.DataAccess;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var webApiUrl = builder.Configuration["WebApi:BaseUrl"];
if (string.IsNullOrWhiteSpace(webApiUrl))
    throw new InvalidOperationException("Missing WebApi:BaseUrl in configuration");


builder.Services.AddRefitClient<IFsw>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(webApiUrl));

builder.Services.AddRefitClient<ITcp>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(webApiUrl));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

