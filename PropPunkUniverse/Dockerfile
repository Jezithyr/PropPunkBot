FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropPunkUniverse/PropPunkUniverse.csproj", "PropPunkUniverse/"]
RUN dotnet restore "PropPunkUniverse/PropPunkUniverse.csproj"
COPY . .
WORKDIR "/src/PropPunkUniverse"
RUN dotnet build "PropPunkUniverse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PropPunkUniverse.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropPunkUniverse.dll"]
