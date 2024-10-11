using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fall2024_Assignment3_tbclements.Models;

public class MultiActorSelection
{
    public int[]? Members { get; set; }

    public string? SelectedActors { get; set; }

    public SelectList ActorList { get; set; } = new SelectList(new List<Actor>(), "Id", "Name");
}
