namespace PeaceKeeper.Database;

public enum TechnologyUse
{
    Civilian = 1 << 0,
    Military = 1 << 1,
    Both = Civilian | Military
}
