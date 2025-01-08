using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.
    AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).
    AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IServiceBase, ServiceBase>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
