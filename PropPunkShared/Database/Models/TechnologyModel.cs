using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("techs"), Index(nameof(Name), IsUnique = true)]
public record TechnologyModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    string Description,
    TechUse Use,
    TechField Field,
    int RawResearchCost
    )
{
    public List<TechnologyModel> RequiredFor { get; } = new ();
    public List<TechnologyModel> Requirements { get; } = new ();
}

public enum TechUse
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

