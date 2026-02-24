FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/YRoute.YBlog.Api/YRoute.YBlog.Api.csproj", "src/YRoute.YBlog.Api/"]
COPY ["src/YRoute.Shared/YRoute.Shared.csproj", "src/YRoute.Shared/"]
RUN dotnet restore "src/YRoute.YBlog.Api/YRoute.YBlog.Api.csproj"
COPY . .
WORKDIR "/src/src/YRoute.YBlog.Api"
RUN dotnet build "YRoute.YBlog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YRoute.YBlog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YRoute.YBlog.Api.dll"]
