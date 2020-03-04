FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "OpenCodeCamp.sln" "OpenCodeCamp.sln"

COPY "BuildingBlocks/EventBus/EventBus/EventBus.csproj" "BuildingBlocks/EventBus/EventBus/EventBus.csproj"
COPY "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj
COPY BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj
COPY BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj
# Marketing service
COPY Services/Marketing/Marketing.UnitTests/Marketing.UnitTests.csproj Services/Marketing/Marketing.UnitTests/Marketing.UnitTests.csproj
COPY Services/Marketing/Marketing.FunctionalTests/Marketing.FunctionalTests.csproj Services/Marketing/Marketing.FunctionalTests/Marketing.FunctionalTests.csproj
COPY Services/Marketing/Marketing.Api/Marketing.Api.csproj Services/Marketing/Marketing.Api/Marketing.Api.csproj
COPY Services/Marketing/Marketing.Domain/Marketing.Domain.csproj Services/Marketing/Marketing.Domain/Marketing.Domain.csproj
COPY Services/Marketing/Marketing.Infrastructure/Marketing.Infrastructure.csproj Services/Marketing/Marketing.Infrastructure/Marketing.Infrastructure.csproj
# Outgoing communications service
COPY Services/OutgoingCommunications/OutgoingCommunications.UnitTests/OutgoingCommunications.UnitTests.csproj Services/OutgoingCommunications/OutgoingCommunications.UnitTests/OutgoingCommunications.UnitTests.csproj
COPY Services/OutgoingCommunications/OutgoingCommunications.FunctionalTests/OutgoingCommunications.FunctionalTests.csproj Services/OutgoingCommunications/OutgoingCommunications.FunctionalTests/OutgoingCommunications.FunctionalTests.csproj
COPY Services/OutgoingCommunications/OutgoingCommunications.Api/OutgoingCommunications.Api.csproj Services/OutgoingCommunications/OutgoingCommunications.Api/OutgoingCommunications.Api.csproj
COPY Services/OutgoingCommunications/OutgoingCommunications.Domain/OutgoingCommunications.Domain.csproj Services/OutgoingCommunications/OutgoingCommunications.Domain/OutgoingCommunications.Domain.csproj
COPY Services/OutgoingCommunications/OutgoingCommunications.Emails/OutgoingCommunications.Emails.csproj Services/OutgoingCommunications/OutgoingCommunications.Emails/OutgoingCommunications.Emails.csproj
COPY Services/OutgoingCommunications/OutgoingCommunications.Infrastructure/OutgoingCommunications.Infrastructure.csproj Services/OutgoingCommunications/OutgoingCommunications.Infrastructure/OutgoingCommunications.Infrastructure.csproj
# Web apps
COPY "Web/WebMvc/WebMvc.csproj" "Web/WebMvc/WebMvc.csproj"
COPY "Web/StaticFiles/StaticFiles.csproj" "Web/StaticFiles/StaticFiles.csproj"

#COPY "docker-compose.dcproj" "docker-compose.dcproj"

#COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "OpenCodeCamp.sln"

COPY . .
WORKDIR /src/src/Web/WebMvc
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMvc.dll"]