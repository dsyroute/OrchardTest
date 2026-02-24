using YRoute.Web.Components;
using YRoute.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// API Clients - configure base URLs from config
builder.Services.AddHttpClient<SolutionsApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:YSolutions"] ?? "http://localhost:5101/"));

builder.Services.AddHttpClient<BlogApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:YBlog"] ?? "http://localhost:5102/"));

builder.Services.AddHttpClient<WikiApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ApiUrls:YWiki"] ?? "http://localhost:5103/"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
