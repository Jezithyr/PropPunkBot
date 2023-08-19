namespace PeaceKeeper.Database;

public enum TechnologyUse
{
    Civilian = 1 << 0,
    Military = 1 << 1,
    Both = Civilian | Military
}

public enum TechField
{
    Generic,
    Infrastructure,
    Industry,
    Materials,
    Economy,
    Strategy,
    Sociology,
    Biology,
    Electronics,
    Physics,
    Mechanics,
    Chemistry,
    Aerospace,
    Nautical,
    Munitions,
    Weaponry
}

public enum GovernmentStatus
{
    InPower,
    Collapsed,
    InExile
}

