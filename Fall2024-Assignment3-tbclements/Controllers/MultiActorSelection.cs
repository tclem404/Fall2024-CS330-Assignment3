using Fall2024_Assignment3_tbclements.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fall2024_Assignment3_tbclements.Controllers;

public class MultiActorSelection
{
    public long[] Members { get; set; }

    public string SelectedActors { get; set; }

    public SelectList ActorList { get; set; } = new SelectList(new List<Actor>(), "Id", "Name");
}
