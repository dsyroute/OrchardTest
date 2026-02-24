# YRoute.MauiApp — .NET MAUI Blazor Hybrid

## Overview

This is the .NET MAUI Blazor Hybrid application for YRoute. It shares Blazor components from `YRoute.Web` and connects to the same microservices.

## Prerequisites

```bash
dotnet workload install maui
```

## Scaffold Command

```bash
cd src
dotnet new maui-blazor -n YRoute.MauiApp -f net8.0
```

## Project Structure

```
YRoute.MauiApp/
├── MauiProgram.cs           # MAUI app entry point
├── MainPage.xaml            # Host page for BlazorWebView
├── wwwroot/                 # Static assets
├── Pages/
│   ├── Solutions/           # Solution pages (shared with Web)
│   ├── Blog/                # Blog pages
│   └── Wiki/                # Wiki pages
├── Services/
│   ├── AppSettings.cs       # Configurable API base URLs
│   ├── SolutionsApiClient.cs
│   ├── BlogApiClient.cs
│   └── WikiApiClient.cs
└── Shared/
    └── NavMenu.razor        # Mobile navigation
```

## MauiProgram.cs Template

```csharp
using Microsoft.Extensions.Logging;
using YRoute.MauiApp.Services;

namespace YRoute.MauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // App settings with configurable base URLs
        var appSettings = new AppSettings();
        builder.Services.AddSingleton(appSettings);

        // API Clients
        builder.Services.AddHttpClient<SolutionsApiClient>(client =>
            client.BaseAddress = new Uri(appSettings.YSolutionsBaseUrl));
        builder.Services.AddHttpClient<BlogApiClient>(client =>
            client.BaseAddress = new Uri(appSettings.YBlogBaseUrl));
        builder.Services.AddHttpClient<WikiApiClient>(client =>
            client.BaseAddress = new Uri(appSettings.YWikiBaseUrl));

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
```

## AppSettings.cs Template

```csharp
namespace YRoute.MauiApp.Services;

public class AppSettings
{
    // TODO: Load from app preferences or platform-specific config
    public string YSolutionsBaseUrl { get; set; } = "https://solutions.yourapi.com/";
    public string YBlogBaseUrl { get; set; } = "https://blog.yourapi.com/";
    public string YWikiBaseUrl { get; set; } = "https://wiki.yourapi.com/";
}
```

## Offline Readiness (TODO)

The `IOfflineCapable` interface from `YRoute.Shared` can be implemented per service:

```csharp
// TODO: Implement caching with Connectivity.NetworkAccess checks
public class SolutionsApiClient : ApiClientBase, IOfflineCapable
{
    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Cache recent solutions to local SQLite or Preferences
        throw new NotImplementedException();
    }
}
```
