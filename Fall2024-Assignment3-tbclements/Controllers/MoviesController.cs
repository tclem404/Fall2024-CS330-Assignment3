using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_tbclements.Data;
using Fall2024_Assignment3_tbclements.Models;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fall2024_Assignment3_tbclements.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        public async Task<IActionResult> GetMoviePoster(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie is null || movie.Poster is null)
            {
                return NotFound();
            }

            var data = movie.Poster;
            return File(data, "image/jpg");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var reviewsAbout = await _context.Review
                .Where(r => r.MovieId == id)
                .ToListAsync();

            var actorsStaring = await _context.MovieActor
                .Include(ma => ma.Actor)
                .Where(ma => ma.MovieId == id)
                .Select(ma => ma.Actor)
                .ToListAsync();

            double avgSent = 0;
            if (reviewsAbout is not null && reviewsAbout.Count() > 0)
            {
                avgSent = reviewsAbout.Average(r => r.ReviewSentiment);
            }
            MovieDetailViewModel vm = new MovieDetailViewModel()
            {
                Actors = actorsStaring,
                Reviews = reviewsAbout,
                Movie = movie,
                AverageSentiment = avgSent
            };

            return View(vm);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            var selList = new SelectList(_context.Actor, "Id", "Name");
            var MAS = new MultiActorSelection()
            {
                ActorList = selList
            };

            var vm = new MovieCreateViewModel()
            {
                Multi = MAS
            };
            return View(vm);
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IMDBLink,Genre,YearOfRelease")] Movie movie, IFormFile? Poster, [Bind("Members")] MultiActorSelection Multi)
        {
            if (Poster is not null && Poster.Length > 0)
            {
                using var stream = new MemoryStream(); // using calls dispose at end of context
                Poster.CopyTo(stream);
                movie.Poster = stream.ToArray();
            }

            // Just override, don't worry
            ModelState.GetValueOrDefault("Multi.Members").ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();

                foreach (int item in Multi.Members)
                { 
                    MovieActor ma = new MovieActor()
                    {
                        MovieId = movie.Id,
                        ActorId = item
                    };

                    _context.Add(ma);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var selList = new SelectList(_context.Actor, "Id", "Name");
            var vm = new MovieCreateViewModel()
            {
                Movie = movie,
                Multi = new MultiActorSelection()
                {
                    ActorList = selList,
                    Members = Multi.Members
                }
            };

            return View(vm);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var listActors = await _context.MovieActor
                .Where(ma => ma.MovieId == movie.Id)
                .Select(ma => ma.ActorId)
                .ToListAsync();



            var vm = new MovieCreateViewModel()
            {
                Movie = movie,
                Poster = movie.Poster,
                Multi = new MultiActorSelection()
                {
                    ActorList = new SelectList(_context.Actor, "Id", "Name"),
                    Members = listActors.ToArray()
                }
            };

            return View(vm);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDBLink,Genre,YearOfRelease")] Movie movie, IFormFile Poster, [Bind("Members")] MultiActorSelection Multi)
        {
            if (id != movie.Id)
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
                        movie.Poster = stream.ToArray();
                    }

                    var allMovieActorPairs = await _context.MovieActor.Where(ma => ma.MovieId == movie.Id).ToListAsync();
                    _context.MovieActor.RemoveRange(allMovieActorPairs);
                    await _context.SaveChangesAsync();

                    foreach (int item in Multi.Members)
                    {
                        MovieActor ma = new MovieActor()
                        {
                            MovieId = movie.Id,
                            ActorId = item
                        };

                        _context.Add(ma);
                    }
                    await _context.SaveChangesAsync();


                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            var selList = new SelectList(_context.Actor, "Id", "Name");
            var vm = new MovieCreateViewModel()
            {
                Movie = movie,
                Multi = new MultiActorSelection()
                {
                    ActorList = selList,
                    Members = Multi.Members
                }
            };

            return View(vm);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            var actorsIn = await _context.MovieActor
                .Where(ma => ma.MovieId == id)
                .ToListAsync();

            _context.RemoveRange(actorsIn);

            var reviewsAbout = await _context.Review
                .Where(r => r.MovieId == id)
                .ToListAsync();

            _context.RemoveRange(reviewsAbout);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
