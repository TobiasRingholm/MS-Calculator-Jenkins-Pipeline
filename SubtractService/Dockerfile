FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SubtractService/SubtractService.csproj", "SubtractService/"]
RUN dotnet restore "SubtractService/SubtractService.csproj"
COPY . .
WORKDIR "/src/SubtractService"
RUN dotnet build "SubtractService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubtractService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubtractService.dll"]
