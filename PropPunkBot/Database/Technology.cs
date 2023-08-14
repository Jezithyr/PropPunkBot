using System.ComponentModel.DataAnnotations;

namespace PropPunkBot.Database;

public class Technology
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
