using System.ComponentModel.DataAnnotations;

namespace IceCreamShop.Models;

public class IceCream
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsDairyFree { get; set; }
}
