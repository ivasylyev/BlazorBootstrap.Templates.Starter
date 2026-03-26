
using Blazored.LocalStorage;
using Dapper;
using NET9.BlazorWebAppServerGlobal.Components;
using NET9.BlazorWebAppServerGlobal.Models;
using NET9.BlazorWebAppServerGlobal.Services;


SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddScoped<IRatesDataService, RatesDataService>();
builder.Services.AddScoped<IGridSettingsService<RateDto>, RatesGridSettingsService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
