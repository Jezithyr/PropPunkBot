namespace PeaceKeeper.Database.Models;

public record ServerSettings(
    long Guild,
    BotEnvironment Environment,
    bool IsCommunityServer,
    string CountryAppsUrl,
    string CompanyAppsUrl);


