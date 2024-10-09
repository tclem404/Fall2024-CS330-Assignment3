using Fall2024_Assignment3_tbclements.Controllers;

namespace Fall2024_Assignment3_tbclements.Models;

public class ActorDetailViewModel
{
    public Actor Actor { get; set; }

    public IEnumerable<Tweet>? Tweets { get; set; }

    public IEnumerable<Movie>? Movies { get; set; }

    public double? avgSentiment { get; set; }
}
