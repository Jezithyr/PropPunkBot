using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("countries"), Index(nameof(Name), IsUnique = true), Index(nameof(ShortName), IsUnique = true)]
public record CountryModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    [property: StringLength(4)] string ShortName
)
{
    [ForeignKey(nameof(Research))]
    public Guid ResearchId { get; set; }
    public CountryResearch Research { get; set; } = default!;

    [ForeignKey(nameof(Government))]
    public Guid GovernmentId { get; set; }
    [Required] public GovernmentModel Government { get; set; } = default!;
}
