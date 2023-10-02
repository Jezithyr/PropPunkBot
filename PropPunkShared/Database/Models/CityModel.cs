using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("cities"), Index(nameof(Name), IsUnique = true), Index(nameof(ShortName), IsUnique = true)]
public record CityModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    [property: StringLength(4)] string ShortName,
    long UrbanPopulation
)
{
    [Required]
    public Guid CountryId { get; set; }

    [Required]
    public CountryModel Country { get; set; } = default!;

    [Required]
    public Guid RegionId { get; set; }

    [Required]
    public RegionModel Region { get; set; } = default!;
}
