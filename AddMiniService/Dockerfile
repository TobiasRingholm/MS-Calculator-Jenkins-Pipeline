FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AddMiniService/AddMiniService.csproj", "AddMiniService/"]
RUN dotnet restore "AddMiniService/AddMiniService.csproj"
COPY . .
WORKDIR "/src/AddMiniService"
RUN dotnet build "AddMiniService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AddMiniService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AddMiniService.dll"]
