using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_tbclements.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    public Movie? Movie { get; set; }

    [ForeignKey(nameof(Movie))]
    public int MovieId { get; set; }

    [Required]
    public string? ReviewText { get; set; }

    public string? Reviewer {  get; set; }

    public string? Publication { get; set; }

    [Required]
    public double ReviewSentiment { get; set; }
}
