namespace PeaceKeeper.Database.Models;

public record Settings(
    long Guild,
    BotEnvironment Environment,
    bool IsCommunityServer,
    string CountryAppsUrl,
    string CompanyAppsUrl);
