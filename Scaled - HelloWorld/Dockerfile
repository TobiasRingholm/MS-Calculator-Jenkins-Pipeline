FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Scaled - HelloWorld/Scaled - HelloWorld.csproj", "Scaled - HelloWorld/"]
RUN dotnet restore "Scaled - HelloWorld/Scaled - HelloWorld.csproj"
COPY . .
WORKDIR "/src/Scaled - HelloWorld"
RUN dotnet build "Scaled - HelloWorld.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Scaled - HelloWorld.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scaled - HelloWorld.dll"]
