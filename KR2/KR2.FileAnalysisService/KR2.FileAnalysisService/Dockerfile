FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["KR2.FileAnalysisService.csproj", "./"]
RUN dotnet restore "KR2.FileAnalysisService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "KR2.FileAnalysisService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KR2.FileAnalysisService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for word cloud storage and ensure proper permissions
RUN mkdir -p /app/WordCloudStorage && chmod 777 /app/WordCloudStorage

ENTRYPOINT ["dotnet", "KR2.FileAnalysisService.dll"]
