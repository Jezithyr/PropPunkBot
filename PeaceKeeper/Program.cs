using Discord;
using PeaceKeeper;

Env.EnsureLoadEnvFile();

var bot = await PeaceKeeperBot.Create();

bot.Client.Log += msg => Console.Out.WriteLineAsync($"[{msg.Severity}]: {msg.Message}");

await bot.Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
await bot.Client.StartAsync();

await Task.Delay(-1);
