using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_tbclements.Models;

public class Movie
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }   

    [Required]
    public string? IMDBLink { get; set; }

    [Required]
    public string? Genre { get; set; }

    [Required]
    public int YearOfRelease { get; set; }

    public byte[]? Poster { get; set; }
}
