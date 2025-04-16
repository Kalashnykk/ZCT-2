using server.Data;
using Microsoft.OpenApi.Models;
using server.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Upload API",
        Version = "v1",
        Description = "API for uploading images",
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFirebase", policy =>
    {
        policy
            .WithOrigins("https://rock-and-stone-e0134.web.app",
            "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<AzureTextRecognitionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "server v1");
    });
}

app.UseRouting();
app.UseCors("AllowFirebase");
app.UseAuthorization();

// Serve static files from the Images folder
var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imageFolderPath),
    RequestPath = "/Images"
});

app.MapControllers();
app.Run();
