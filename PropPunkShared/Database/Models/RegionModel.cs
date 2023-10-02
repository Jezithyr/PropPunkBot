using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("regions"), Index(nameof(Name), IsUnique = true), Index(nameof(ShortName), IsUnique = true)]
public record RegionModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    [property: StringLength(4)] string ShortName,
    long RuralPopulation
)
{
    [Required]
    public Guid CountryId { get; set; }

    [Required]
    public CountryModel Country { get; set; } = default!;
}
