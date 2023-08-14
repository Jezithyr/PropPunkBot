using Dapper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Npgsql;
using PropPunkBot;

Env.EnsureLoadEnvFile();

var connString = new NpgsqlConnectionStringBuilder()
{
    Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
    Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
    Database = Environment.GetEnvironmentVariable("DATABASE_NAME"),
    Username = Environment.GetEnvironmentVariable("DATABASE_USERNAME"),
    Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
}.ConnectionString;

await using var connection = new NpgsqlConnection(connString);
var techs = await connection.QueryAsync("SELECT * FROM technologies");
Console.WriteLine(techs.ToList());

var client = new DiscordSocketClient();
var commands = new CommandHandler(client, new CommandService());
await commands.InstallCommandsAsync();

client.Log += msg => Console.Out.WriteLineAsync($"[{msg.Severity}]: {msg.Message}");

await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
await client.StartAsync();

await Task.Delay(-1);
