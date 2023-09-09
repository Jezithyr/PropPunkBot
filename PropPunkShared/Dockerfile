FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropPunkShared/PropPunkShared.csproj", "PropPunkShared/"]
RUN dotnet restore "PropPunkShared/PropPunkShared.csproj"
COPY . .
WORKDIR "/src/PropPunkShared"
RUN dotnet build "PropPunkShared.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PropPunkShared.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropPunkShared.dll"]
