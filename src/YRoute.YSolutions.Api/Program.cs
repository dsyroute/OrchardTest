using MongoDB.Driver;
using YRoute.YSolutions.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>() ?? new MongoDbSettings();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));

// Repositories
builder.Services.AddScoped<ISolutionRepository, SolutionRepository>();
builder.Services.AddScoped<ISolutionCategoryRepository, SolutionCategoryRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "YRoute.YSolutions API", Version = "v1" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
