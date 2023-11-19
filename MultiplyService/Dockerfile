FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MultiplyService/MultiplyService.csproj", "MultiplyService/"]
COPY ["Monitoring/Monitoring.csproj", "Monitoring/"]
RUN dotnet restore "MultiplyService/MultiplyService.csproj"
COPY . .
WORKDIR "/src/MultiplyService"
RUN dotnet build "MultiplyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MultiplyService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MultiplyService.dll"]
