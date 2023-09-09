namespace PropPunkShared.Data.Models;

public record ServerSettings(
    long Guild,
    BotEnvironment Environment,
    bool IsCommunityServer,
    string CountryAppsUrl,
    string CompanyAppsUrl);

public enum BotEnvironment
{
    CommunityServer = 0,
    LiveServer,
    Development,
}
