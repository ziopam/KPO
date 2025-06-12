FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KR3.OrderService/KR3.OrderService.csproj", "KR3.OrderService/"]
COPY ["KR3.Shared/KR3.Shared.csproj", "KR3.Shared/"]
RUN dotnet restore "KR3.OrderService/KR3.OrderService.csproj"
COPY . .
WORKDIR "/src/KR3.OrderService"
RUN dotnet build "KR3.OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KR3.OrderService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KR3.OrderService.dll"]
