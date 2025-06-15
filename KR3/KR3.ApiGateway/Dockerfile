FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Install curl for Docker health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KR3.ApiGateway/KR3.ApiGateway.csproj", "KR3.ApiGateway/"]
COPY ["KR3.Shared/KR3.Shared.csproj", "KR3.Shared/"]
RUN dotnet restore "KR3.ApiGateway/KR3.ApiGateway.csproj"
COPY . .
WORKDIR "/src/KR3.ApiGateway"
RUN dotnet build "KR3.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KR3.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KR3.ApiGateway.dll"]
