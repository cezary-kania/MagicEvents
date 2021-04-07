FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MagicEvents.Api.Service/src/MagicEvents.Api.Service.Api/MagicEvents.Api.Service.Api.csproj", "MagicEvents.Api.Service/src/MagicEvents.Api.Service.Api/"]
COPY ["MagicEvents.Api.Service/src/MagicEvents.Api.Service.Application/MagicEvents.Api.Service.Application.csproj", "MagicEvents.Api.Service/src/MagicEvents.Api.Service.Application/"]
COPY ["MagicEvents.Api.Service/src/MagicEvents.Api.Service.Domain/MagicEvents.Api.Service.Domain.csproj", "MagicEvents.Api.Service/src/MagicEvents.Api.Service.Domain/"]
COPY ["MagicEvents.Api.Service/src/MagicEvents.Api.Service.Infrastructure/MagicEvents.Api.Service.Infrastructure.csproj", "MagicEvents.Api.Service/src/MagicEvents.Api.Service.Infrastructure/"]
RUN dotnet restore "MagicEvents.Api.Service/src/MagicEvents.Api.Service.Api/MagicEvents.Api.Service.Api.csproj"
COPY . .
WORKDIR "/src/MagicEvents.Api.Service/src/MagicEvents.Api.Service.Api"
RUN dotnet build "MagicEvents.Api.Service.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagicEvents.Api.Service.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicEvents.Api.Service.Api.dll"]
