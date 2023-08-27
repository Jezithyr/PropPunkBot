using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    private readonly DbService _db;
    private readonly SettingsService _settings;
    private readonly UserService _users;
    private readonly TechService _tech;
    public ResearchService(DbService db, SettingsService settings, UserService users, TechService tech)
    {
        _db = db;
        _settings = settings;
        _users = users;
        _tech = tech;
    }





}
