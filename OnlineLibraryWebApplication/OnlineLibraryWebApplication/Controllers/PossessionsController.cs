using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Controllers
{
    public class PossessionsController : Controller
    {
        private readonly DblibraryContext _context;

        public PossessionsController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Possessions
        public async Task<IActionResult> Index()
        {
            var dblibraryContext = _context.Possessions.Include(p => p.Book).Include(p => p.User);
            return View(await dblibraryContext.ToListAsync());
        }

        // POST: Possessions/AddToLibrary/5
        [HttpPost]
        public async Task<IActionResult> AddToLibrary(int id)
        {
            // Перевірка, чи книга вже є у бібліотеці користувача
            var existingPossession = await _context.Possessions
                .Where(p => p.BookId == id)
                .FirstOrDefaultAsync();

            if (existingPossession == null)
            {
                // Якщо книги немає у бібліотеці, додати її
                _context.Possessions.Add(new Possession { BookId = id });
                await _context.SaveChangesAsync();
            }

            // Повернення до сторінки з книгою
            return RedirectToAction("Details", "Books", new { id });
        }

        // GET: Possessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var possession = await _context.Possessions
                .Include(p => p.Book)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (possession == null)
            {
                return NotFound();
            }

            return View(possession);
        }

        // GET: Possessions/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Possessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,BookId,Accessibility,StartTime,EndTime,CurrentPage")] Possession possession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(possession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description", possession.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", possession.UserId);
            return View(possession);
        }

        // GET: Possessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var possession = await _context.Possessions.FindAsync(id);
            if (possession == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description", possession.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", possession.UserId);
            return View(possession);
        }

        // POST: Possessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,BookId,Accessibility,StartTime,EndTime,CurrentPage")] Possession possession)
        {
            if (id != possession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(possession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PossessionExists(possession.Id))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description", possession.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", possession.UserId);
            return View(possession);
        }

        // GET: Possessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var possession = await _context.Possessions
                .Include(p => p.Book)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (possession == null)
            {
                return NotFound();
            }

            return View(possession);
        }

        // POST: Possessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var possession = await _context.Possessions.FindAsync(id);
            if (possession != null)
            {
                _context.Possessions.Remove(possession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PossessionExists(int id)
        {
            return _context.Possessions.Any(e => e.Id == id);
        }
    }
}
