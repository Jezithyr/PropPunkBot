FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropPunkBot/PropPunkBot.csproj", "PropPunkBot/"]
RUN dotnet restore "PropPunkBot/PropPunkBot.csproj"
COPY . .
WORKDIR "/src/PropPunkBot"
RUN dotnet build "PropPunkBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PropPunkBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropPunkBot.dll"]
