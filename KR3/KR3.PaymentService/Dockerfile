FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KR3.PaymentService/KR3.PaymentService.csproj", "KR3.PaymentService/"]
COPY ["KR3.Shared/KR3.Shared.csproj", "KR3.Shared/"]
RUN dotnet restore "KR3.PaymentService/KR3.PaymentService.csproj"
COPY . .
WORKDIR "/src/KR3.PaymentService"
RUN dotnet build "KR3.PaymentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KR3.PaymentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KR3.PaymentService.dll"]
