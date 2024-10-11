using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_tbclements.Data;
using Fall2024_Assignment3_tbclements.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenAI.Chat;
using System.ClientModel;
using Azure.AI.OpenAI;

namespace Fall2024_Assignment3_tbclements.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ActorsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        public async Task<IActionResult> GetActorPhoto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Actor.FindAsync(id);
            if (student is null || student.Poster is null)
            {
                return NotFound();
            }

            var data = student.Poster;
            return File(data, "image/jpg");
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            var tweetsAbout = await _context.Tweet
                .Where(t => t.ActorId == id)
                .ToListAsync();

            var moviesIn = await _context.MovieActor
                .Include(ma => ma.Movie)
                .Where(ma => ma.ActorId == id)
                .Select(ma => ma.Movie)
                .ToListAsync();

            double averageSent = 0;
            if(tweetsAbout is not null && tweetsAbout.Count() > 0)
            {
                averageSent = tweetsAbout.Average(t => t.TweetSentiment);
            }

            var vm = new ActorDetailViewModel()
            {
                Actor = actor,
                Tweets = tweetsAbout,
                Movies = moviesIn,
                avgSentiment = averageSent
                
            };

            return View(vm);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            var selList = new SelectList(_context.Movie, "Id", "Title");
            var MAS = new MultiMovieSelection()
            {
                MovieList = selList
            };

            var vm = new ActorCreateViewModel()
            {
                Multi = MAS
            };
            return View(vm);
        }

        private async void generateTweets(Actor actor) {

            ApiKeyCredential ApiCredential = new(_configuration.GetValue(typeof(string), "OpenAIKey") as string);

            string AiDeployment = "gpt-35-turbo";
            ChatClient client = new AzureOpenAIClient(new Uri(_configuration.GetValue(typeof(string), "OpenAIEndpoint") as string), ApiCredential).GetChatClient(AiDeployment);

            ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");

            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        }


        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile? Poster, [Bind("Members")] MultiMovieSelection Multi)
        {
            if (Poster is not null && Poster.Length > 0)
            {
                using var stream = new MemoryStream(); // using calls dispose at end of context
                Poster.CopyTo(stream);
                actor.Poster = stream.ToArray();
            }

            // Just override, don't worry
            ModelState.GetValueOrDefault("Multi.Members").ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                
                _context.Add(actor);
                await _context.SaveChangesAsync();

                foreach (int item in Multi.Members)
                {
                    MovieActor ma = new MovieActor()
                    {
                        ActorId = actor.Id,
                        MovieId = item
                    };

                    _context.Add(ma);
                }

                await _context.SaveChangesAsync();

                generateTweets(actor);

                return RedirectToAction(nameof(Index));
            }

            var selList = new SelectList(_context.Movie, "Id", "Title");
            var vm = new ActorCreateViewModel()
            {
                Actor = actor,
                Multi = new MultiMovieSelection()
                {
                    MovieList = selList,
                    Members = Multi.Members
                }
            };

            return View(vm);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            var listMovies = await _context.MovieActor
                .Where(ma => ma.ActorId == actor.Id)
                .Select(ma => ma.MovieId)
                .ToListAsync();



            var vm = new ActorCreateViewModel()
            {
                Actor = actor,
                Poster = actor.Poster,
                Multi = new MultiMovieSelection()
                {
                    MovieList = new SelectList(_context.Movie, "Id", "Title"),
                    Members = listMovies.ToArray()
                }
            };

            return View(vm);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile Poster, [Bind("Members")] MultiActorSelection Multi)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            // Just override, don't worry
            ModelState.GetValueOrDefault("Multi.Members").ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                try
                {
                    if (Poster is not null && Poster.Length > 0)
                    {
                        using var stream = new MemoryStream(); // using calls dispose at end of context
                        Poster.CopyTo(stream);
                        actor.Poster = stream.ToArray();
                    }

                    var allMovieActorPairs = await _context.MovieActor.Where(ma => ma.ActorId == actor.Id).ToListAsync();
                    _context.MovieActor.RemoveRange(allMovieActorPairs);
                    await _context.SaveChangesAsync();

                    foreach (int item in Multi.Members)
                    {
                        MovieActor ma = new MovieActor()
                        {
                            ActorId = actor.Id,
                            MovieId = item
                        };

                        _context.Add(ma);
                    }
                    await _context.SaveChangesAsync();

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var listMovies = await _context.MovieActor
                .Where(ma => ma.ActorId == actor.Id)
                .Select(ma => ma.MovieId)
                .ToListAsync();



            var vm = new ActorCreateViewModel()
            {
                Actor = actor,
                Poster = actor.Poster,
                Multi = new MultiMovieSelection()
                {
                    MovieList = new SelectList(_context.Movie, "Id", "Title"),
                    Members = listMovies.ToArray()
                }
            };

            return View(vm);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            var tweets = await _context.Tweet
                .Where(t => t.ActorId == id)
                .ToListAsync();

            _context.RemoveRange(tweets);

            var moviesIn = await _context.MovieActor
                .Where(ma => ma.ActorId == id)
                .ToListAsync();

            _context.RemoveRange(moviesIn);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
