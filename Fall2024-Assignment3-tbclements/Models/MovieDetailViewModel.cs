namespace Fall2024_Assignment3_tbclements.Models;

public class MovieDetailViewModel
{
    public Movie Movie { get; set; }

    public IEnumerable<Review>? Reviews { get; set; }

    public IEnumerable<Actor>? Actors { get; set; }

    public double AverageSentiment { get; set; }
}
