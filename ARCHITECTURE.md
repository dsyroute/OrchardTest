# YRoute – Architekturentscheidung

## Gesamtarchitektur

```
┌─────────────────────────────────────────────────────────┐
│                    Clients                              │
│  YRoute.Web (Blazor Server)  │  YRoute.MauiApp (MAUI)  │
└────────────────┬────────────────────────────────────────┘
                 │ HTTP/REST
        ┌────────▼─────────────────────────────────┐
        │           API Gateway (optional)          │
        │    (z.B. YARP Reverse Proxy / NGINX)      │
        └────┬──────────┬──────────────┬────────────┘
             │          │              │
     ┌───────▼──┐  ┌────▼───┐  ┌─────▼──────┐
     │  YSolut. │  │ YBlog  │  │   YWiki    │
     │   .Api   │  │  .Api  │  │    .Api    │
     │ :5101    │  │ :5102  │  │   :5103    │
     └──────┬───┘  └───┬────┘  └────┬───────┘
            │          │            │
            └──────────┼────────────┘
                       │
                  ┌────▼────┐
                  │ MongoDB │
                  └─────────┘

        ┌────────────────────┐
        │  YRoute.OrchardHost│
        │  (CMS / Editorial) │
        │  :5000             │
        └────────┬───────────┘
                 │
           ┌─────▼──────┐
           │ PostgreSQL  │
           └────────────┘
```

## Technologieentscheidungen

| Bereich | Technologie | Begründung |
|---------|-------------|------------|
| CMS / Redaktion | Orchard Core 2.x | Bewährtes .NET CMS, PostgreSQL-ready, Themes/Menüs/SEO |
| Orchard DB | PostgreSQL | Stabile RDBMS, Orchard Core optimal unterstützt |
| Microservice-Daten | MongoDB | Dokumentenmodell optimal für Blog/Wiki/Solution-Content |
| Web-Frontend | Blazor Server | C# End-to-End, kein JS-Framework nötig |
| Mobile/Desktop | MAUI Blazor Hybrid | Wiederverwendung von Blazor-Komponenten, Native Shell |
| API-Stil | REST/JSON | Pragmatisch, einfach konsumierbar |

## Abgrenzung: Orchard Core vs. Microservices

| | Orchard Core (PostgreSQL) | Microservices (MongoDB) |
|---|---|---|
| **Zweck** | Redaktionelle Seiten, Landingpages, Navigation | Strukturierte Fachinhalte |
| **Inhalte** | Startseite, Über uns, Kontakt, Menüs | Blog-Posts, Wiki-Artikel, Lösungen |
| **SEO** | Orchard-native URL-Aliase, Meta-Felder | Slug-basiert, ggf. per Orchard referenziert |
| **Media** | Orchard Media Library | Referenzierte URLs (aus Orchard oder CDN) |
| **Mehrsprachigkeit** | OrchardCore.Localization | TODO: i18n-Felder im Dokumentenmodell |

## Integrationsstrategie (pragmatisch)

1. **Orchard referenziert Microservice-Inhalte per iFrame / Custom Widget** (einfachste Option)
2. **Orchard Custom Module** ruft Microservice-API auf und rendert Inhalte
3. **Read-Model Sync**: Microservices schreiben Summary-Daten in Orchard (via Webhook)

Empfehlung für Phase 1: Option 2 (Custom Orchard Module per Bereich).

## Namespace-Konventionen

- `YRoute.YSolutions.*` – Lösungen-Microservice
- `YRoute.YBlog.*` – Blog-Microservice
- `YRoute.YWiki.*` – Wiki-Microservice
- `YRoute.Web.*` – Blazor Frontend
- `YRoute.Shared.*` – Gemeinsame Contracts/Interfaces
- `YRoute.MauiApp.*` – MAUI Blazor Hybrid App

## Lokale Entwicklung

```bash
# Infrastruktur starten (PostgreSQL + MongoDB)
docker-compose up -d postgres mongodb

# Einzelne Services starten
dotnet run --project src/YRoute.YSolutions.Api   # :5101
dotnet run --project src/YRoute.YBlog.Api        # :5102
dotnet run --project src/YRoute.YWiki.Api        # :5103
dotnet run --project src/YRoute.OrchardHost      # :5000
dotnet run --project src/YRoute.Web              # :5200
```

## Azure-Deployment (Skizze)

- **Container Apps**: Jeder Microservice als einzelner Container
- **Azure Database for PostgreSQL**: Managed Orchard-Datenbank
- **Azure Cosmos DB for MongoDB**: Managed MongoDB-kompatibel
- **Azure App Configuration**: Zentrale Konfiguration
- **Azure Key Vault**: Secrets (Connection Strings, API Keys)
- **.NET Aspire**: Lokale Orchestrierung + Azure Deployment Integration

## TODO-Marker

- [ ] Authentication/Authorization (z.B. OpenIddict in Orchard, JWT für APIs)
- [ ] Rate Limiting auf den API-Endpunkten
- [ ] MAUI App vollständig scaffolden (workload install maui)
- [ ] Orchard Custom Module für Microservice-Integration
- [ ] .NET Aspire AppHost für vollständige lokale Orchestrierung
- [ ] i18n / Mehrsprachigkeit in Microservices
- [ ] Full-text Search (z.B. MongoDB Atlas Search oder Elasticsearch)
- [ ] CI/CD Pipeline (GitHub Actions)
