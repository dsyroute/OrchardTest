FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/YRoute.OrchardHost/YRoute.OrchardHost.csproj", "src/YRoute.OrchardHost/"]
RUN dotnet restore "src/YRoute.OrchardHost/YRoute.OrchardHost.csproj"
COPY . .
WORKDIR "/src/src/YRoute.OrchardHost"
RUN dotnet build "YRoute.OrchardHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YRoute.OrchardHost.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YRoute.OrchardHost.dll"]
