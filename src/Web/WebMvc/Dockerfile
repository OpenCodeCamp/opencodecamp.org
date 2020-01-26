FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# Keep the project list and command dotnet restore identical in all Dockfiles to maximize image cache utilization
COPY OpenCodeCamp-ServicesAndWebApps.sln .
#COPY docker-compose.dcproj /src/
COPY src/BuildingBlocks/EventBus/EventBus/EventBus.csproj src/BuildingBlocks/EventBus/EventBus/
COPY src/BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj src/BuildingBlocks/EventBus/EventBusRabbitMQ/
COPY src/BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj src/BuildingBlocks/EventBus/EventBusServiceBus/
COPY src/BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj src/BuildingBlocks/EventBus/IntegrationEventLogEF/
COPY src/BuildingBlocks/WebHostCustomization/WebHost.Customization/WebHost.Customization.csproj src/BuildingBlocks/WebHostCustomization/WebHost.Customization/
# Marketing service
COPY src/Services/Marketing/Marketing.UnitTests/Marketing.UnitTests.csproj src/Services/Marketing/Marketing.UnitTests/
COPY src/Services/Marketing/Marketing.FunctionalTests/Marketing.FunctionalTests.csproj src/Services/Marketing/Marketing.FunctionalTests/
COPY src/Services/Marketing/Marketing.Api/Marketing.Api.csproj src/Services/Marketing/Marketing.Api/
COPY src/Services/Marketing/Marketing.Domain/Marketing.Domain.csproj src/Services/Marketing/Marketing.Domain/
COPY src/Services/Marketing/Marketing.Infrastructure/Marketing.Infrastructure.csproj src/Services/Marketing/Marketing.Infrastructure/
# Outgoing communications service
COPY src/Services/OutgoingCommunications/OutgoingCommunications.UnitTests/OutgoingCommunications.UnitTests.csproj src/Services/OutgoingCommunications/OutgoingCommunications.UnitTests/
COPY src/Services/OutgoingCommunications/OutgoingCommunications.FunctionalTests/OutgoingCommunications.FunctionalTests.csproj src/Services/OutgoingCommunications/OutgoingCommunications.FunctionalTests/
COPY src/Services/OutgoingCommunications/OutgoingCommunications.Api/OutgoingCommunications.Api.csproj src/Services/OutgoingCommunications/OutgoingCommunications.Api/
COPY src/Services/OutgoingCommunications/OutgoingCommunications.Domain/OutgoingCommunications.Domain.csproj src/Services/OutgoingCommunications/OutgoingCommunications.Domain/
COPY src/Services/OutgoingCommunications/OutgoingCommunications.Infrastructure/OutgoingCommunications.Infrastructure.csproj src/Services/OutgoingCommunications/OutgoingCommunications.Infrastructure/
# Web apps
COPY src/Web/WebMvc/WebMvc.csproj src/Web/WebMvc/

RUN dotnet restore OpenCodeCamp-ServicesAndWebApps.sln

COPY . .
WORKDIR /src/src/Web/WebMvc
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMvc.dll"]