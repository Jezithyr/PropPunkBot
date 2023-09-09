using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("countries"), Index(nameof(Name), IsUnique = true), Index(nameof(ShortName), IsUnique = true)]
public record Country(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    [property: StringLength(128)] string ShortName
);
