using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("governments"), Index(nameof(Name), IsUnique = true)]
public record GovernmentModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name)
{
    public List<CountryModel> Countries { get; set; } = default!;
}
