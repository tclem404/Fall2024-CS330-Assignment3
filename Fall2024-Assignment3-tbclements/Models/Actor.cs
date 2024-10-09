using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_tbclements.Models;

public class Actor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Name  { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    public int Age  { get; set; }

    [Required]
    public string? IMDBLink { get; set; }

    public byte[]? Poster { get; set; }
}
