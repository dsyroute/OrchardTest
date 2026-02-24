using MongoDB.Driver;
using YRoute.YBlog.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>() ?? new MongoDbSettings();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));

builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "YRoute.YBlog API", Version = "v1" });
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
