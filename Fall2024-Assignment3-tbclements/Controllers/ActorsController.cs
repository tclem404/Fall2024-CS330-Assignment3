using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_tbclements.Data;
using Fall2024_Assignment3_tbclements.Models;

namespace Fall2024_Assignment3_tbclements.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
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
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile? Poster)
        {
            if (ModelState.IsValid)
            {
                if (Poster is not null && Poster.Length > 0)
                {
                    using var stream = new MemoryStream(); // using calls dispose at end of context
                    Poster.CopyTo(stream);
                    actor.Poster = stream.ToArray();
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
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
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDBLink")] Actor actor, IFormFile Poster)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

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
            return View(actor);
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
