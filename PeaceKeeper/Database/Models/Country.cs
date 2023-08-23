namespace PeaceKeeper.Database.Models;

public record Country(
    Guid Id,
    string Name,
    string ShortName,
    string? FlagUrl,
    string? WikiUrl);
