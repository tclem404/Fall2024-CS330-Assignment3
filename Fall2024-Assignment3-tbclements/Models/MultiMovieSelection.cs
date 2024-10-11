using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fall2024_Assignment3_tbclements.Models;

public class MultiMovieSelection
{
    public int[]? Members { get; set; }

    public string? SelectedMovies { get; set; }

    public SelectList MovieList { get; set; } = new SelectList(new List<Actor>(), "Id", "Title");
}

