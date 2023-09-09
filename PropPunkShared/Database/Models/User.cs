using System.ComponentModel.DataAnnotations.Schema;

namespace PropPunkShared.Database.Models;

public record User(long Id, long DiscordId, string Name);
