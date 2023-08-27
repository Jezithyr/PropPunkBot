using Dapper;
using Discord.Interactions;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createtech", "create a new technology")]
    public async Task CreateTechnology(string techName, TechnologyUse uses, int yearDeveloped, TechField field, int cost)
    {
        await DeferAsync();
        if (techName.Length > 128)
        {
            await FollowupAsync("Technology name is too long, must be less than 128 characters!");
            return;
        }

        if (await _tech.Create(techName, uses, yearDeveloped, field, cost))
        {
            await FollowupAsync($"Technology {techName} Created!");
            return;
        }
        await FollowupAsync($"Technology {techName} Already Exists!");
    }
    [SlashCommand("removetech", "remove a technology")]
    public async Task RemoveTech(string techName)
    {
        await DeferAsync();
        if (techName.Length > 128)
        {
            await FollowupAsync("Technology name is too long, must be less than 128 characters!");
            return;
        }
        if (await _tech.Remove(techName))
        {
            await FollowupAsync($"Technology {techName} Removed!");
            return;
        }
        await FollowupAsync($"Technology {techName} Does not Exist!");
    }
}
