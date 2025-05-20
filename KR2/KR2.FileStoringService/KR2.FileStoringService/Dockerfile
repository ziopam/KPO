FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["KR2.FileStoringService.csproj", "./"]
RUN dotnet restore "KR2.FileStoringService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "KR2.FileStoringService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KR2.FileStoringService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for file storage and ensure proper permissions
RUN mkdir -p /app/FileStorage && chmod 777 /app/FileStorage

ENTRYPOINT ["dotnet", "KR2.FileStoringService.dll"]
