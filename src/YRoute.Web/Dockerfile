FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/YRoute.Web/YRoute.Web.csproj", "src/YRoute.Web/"]
COPY ["src/YRoute.Shared/YRoute.Shared.csproj", "src/YRoute.Shared/"]
RUN dotnet restore "src/YRoute.Web/YRoute.Web.csproj"
COPY . .
WORKDIR "/src/src/YRoute.Web"
RUN dotnet build "YRoute.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YRoute.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YRoute.Web.dll"]
