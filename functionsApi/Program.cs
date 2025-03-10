using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.BlobService;
using Services.HttpService;
using Services.KnapsackService;
using Services.SpotifyService;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddScoped<IKnapsackService, KnapsackService>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();



builder.Build().Run();
