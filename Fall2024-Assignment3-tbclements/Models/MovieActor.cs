using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_tbclements.Models;

public class MovieActor
{
    [Key]
    public int Id { get; set; }

    public Movie? Movie { get; set; }

    public Actor? Actor { get; set; }

    [ForeignKey(nameof(Actor))]
    public int ActorId { get; set; }

    [ForeignKey(nameof(Movie))]
    public int MovieId { get; set; }
}
