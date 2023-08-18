using System.ComponentModel.DataAnnotations;

namespace PeaceKeeper.Database;

public class Technology
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
