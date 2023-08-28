using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class NewsService : PeacekeeperServiceBase
{

    public async Task CreateNewsArticle(string organization, NewsType newsType, string text,
        string? imageLink = null,string? organizationIconLink = null, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        await connection.QuerySingleAsync<int>(
            "INSERT INTO news(organization, newstype, date, text, imagelink, organizationiconlink) " +
            "VALUES(@org,@newstype, @date, @story, @image, @icon) ON CONFLICT DO NOTHING RETURNING -1",
            new
            {
                org = organization, newstype = (int) newsType, story = text, image = imageLink,
                icon = organizationIconLink
            }
        );
    }

    public async Task<bool> Exists(Guid articleId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return (await connection.QuerySingleOrDefaultAsync<News>(
            "SELECT * FROM news WHERE id = @id LIMIT 1",
            new {id = articleId}
        )) != null;
    }

    public async Task<News?> Get(Guid articleId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<News>(
            "SELECT * FROM news WHERE id = @id LIMIT 1",
            new {id = articleId}
        );
    }

    public async Task<bool> Delete(Guid articleId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        if (!await Exists(articleId, connection))
            return false;
        await connection.QueryAsync(
            "DELETE FROM news WHERE id = @id",
            new {id = articleId}
        );
        return true;
    }

    public NewsService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState) : base(settings, perms, users, db, worldState)
    {
    }
}
