namespace PeaceKeeper.Database.Models;

public record WorldState(
    int Lock,
    string StartDate,
    int Year,
    int Quarter
    );
