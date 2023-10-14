using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("countries_research")]
public record CountryResearch(
    int PointOverflow
)
{
    [property: Key, ForeignKey(nameof(Country))]
    public Guid CountryId { get; set; }
    public CountryModel Country { get; set; } = default!;
    public List<CountryResearchModifier> ResearchMods { get; set; } = default!;
    public List<CountryResearchSlot> ResearchSlots { get; set; } = default!;
};

[Table("countries_research_mods"), Index(nameof(CountryId), nameof(Field), IsUnique = true)]
public record CountryResearchModifier(
    TechField Field,
    float Modifier
)
{
    [Key, ForeignKey(nameof(Country))] public Guid CountryId { get; set; }
    CountryModel Country { get; set; } = default!;
}

[Table("countries_research_slots"), Index(nameof(CountryId))]
public record CountryResearchSlot(
    [property: Key] Guid Id,
    int PointProgress
)
{
    [ForeignKey(nameof(Country))]
    public Guid CountryId { get; set; }

    public CountryModel Country { get; set; } = default!;
    [ForeignKey(nameof(Tech))]
    public Guid TechId{ get; set; }
    public TechnologyModel Tech{ get; set; } = default!;
}

[Table("country_research_progress"), Index(nameof(CountryId), nameof(Tech),
     IsUnique = true)]
public record CountryResearchProgress(
    float Percentage,
    bool FinishedResearch
)
{
    [Key, ForeignKey(nameof(Country))] public Guid CountryId { get; set; }
    public CountryModel Country { get; set; } = default!;
    [property: ForeignKey(nameof(Tech))] public Guid TechId { get; set; }
    public TechnologyModel Tech { get; set; } = default!;
}


//TODO: company research
