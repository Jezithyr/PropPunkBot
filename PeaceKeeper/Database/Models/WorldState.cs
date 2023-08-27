namespace PeaceKeeper.Database.Models;

public record WorldStateRaw(
    int Lock,
    string StartDate,
    int Year,
    int Quarter
    );
public record WorldState(
    DateTime StartDate,
    int Year,
    int Quarter
);
