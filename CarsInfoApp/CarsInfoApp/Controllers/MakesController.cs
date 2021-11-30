namespace CarsInfoApp.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using CarsInfoApp.Models;

    public class MakesController : BaseController
    {
        // GET: Makes
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var makesQuery = from m in this._context.Makes
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                makesQuery = makesQuery
                    .Where(m => m.Name.Contains(searchString) || m.TotalIncome.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    makesQuery = makesQuery.OrderByDescending(m => m.Name);
                    break;
                case "Date":
                    makesQuery = makesQuery.OrderBy(m => m.EstablishmentDate);
                    break;
                case "date_desc":
                    makesQuery = makesQuery.OrderByDescending(m => m.EstablishmentDate);
                    break;
                default:
                    makesQuery = makesQuery.OrderBy(m => m.Name);
                    break;
            }

            return View(makesQuery.ToList());
        }

        // GET: Makes/Details/{makeId}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var make = await _context.Makes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (make == null)
            {
                return NotFound();
            }

            return View(make);
        }

        // GET: Makes/Create
        public IActionResult Create()
        {
            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Makes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,EstablishmentDate,TotalCarsMade,TotalIncome")] Make make)
        {
            if (ModelState.IsValid)
            {
                make.Id = Guid.NewGuid();
                _context.Add(make);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(make);
        }

        // GET: Makes/Edit/{makeId}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var make = await _context.Makes.FindAsync(id);
                if (make == null)
                {
                    return NotFound();
                }

                return View(make);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Makes/Edit/{makeId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,EstablishmentDate,TotalCarsMade,TotalIncome")] Make make)
        {
            if (id != make.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(make);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MakeExists(make.Id))
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
            return View(make);
        }

        // GET: Makes/Delete/{makeId}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var make = await _context.Makes
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (make == null)
                {
                    return NotFound();
                }

                return View(make);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Makes/Delete/{makeId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var make = await _context.Makes.FindAsync(id);
            var models = this._context.Models.Where(x => x.MakeId == make.Id);

            foreach (var model in models.ToList())
            {
                var engines = this._context.Engines
                    .Where(x => x.ModelId == model.Id);

                foreach (var engine in engines.ToList())
                {
                    this._context.Engines.Remove(engine);
                }

                this._context.Remove(model);
            }

            _context.Makes.Remove(make);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MakeExists(Guid id)
        {
            return _context.Makes.Any(e => e.Id == id);
        }
    }
}
