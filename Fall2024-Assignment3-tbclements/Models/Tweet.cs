using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_tbclements.Models;

public class Tweet
{
    [Key]
    public int Id { get; set; }

    public Actor? Actor { get; set; }

    [ForeignKey(nameof(Actor))]
    public int ActorId { get; set; }

    [Required]
    public string? TweetText { get; set; }

    [Required]
    public double TweetSentiment { get; set; }
}
