namespace CarsInfoApp.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using CarsInfoApp.Models;
    using CarsInfoApp.Utils;
    using Microsoft.AspNetCore.Authorization;

    public class ModelsController : BaseController
    {
        // GET: Models
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ManufacturerSortParm = sortOrder == "manufacturer" ? "manufacturer_desc" : "manufacturer";
            var modelsQuery = from m in this._context.Models
                              select m;

            foreach (var model in modelsQuery.ToList())
            {
                var make = this._context.Makes.FirstOrDefault(x => x.Id == model.MakeId);
                model.Make = make;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                modelsQuery = modelsQuery.Where(m => m.ModelName.Contains(searchString) || m.Make.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    modelsQuery = modelsQuery.OrderByDescending(m => m.ModelName);
                    break;
                case "manufacturer":
                    modelsQuery = modelsQuery.OrderBy(m => m.Make);
                    break;
                case "manufacturer_desc":
                    modelsQuery = modelsQuery.OrderByDescending(m => m.Make);
                    break;
                default:
                    modelsQuery = modelsQuery.OrderBy(m => m.ModelName);
                    break;
            }

            return View(modelsQuery.ToList());
        }

        // GET: Models/Details/{modelId}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var model = await _context.Models
                    .Include(m => m.Make)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Models/Create
        public IActionResult Create()
        {
            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                ViewData["MakeId"] = new SelectList(_context.Makes, "Id", "Name");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Models/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MakeId,ModelName,StartYear,EndYear,Price,TotalModelsMade")] Model model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MakeId"] = new SelectList(_context.Makes, "Id", "Name", model.MakeId);
            return View(model);
        }

        // GET: Models/Edit/{modelId}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var model = await _context.Models.FindAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
                ViewData["MakeId"] = new SelectList(_context.Makes, "Id", "Name", model.MakeId);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Models/Edit/{modelId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MakeId,ModelName,StartYear,EndYear,Price,TotalModelsMade")] Model model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelExists(model.Id))
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
            ViewData["MakeId"] = new SelectList(_context.Makes, "Id", "Name", model.MakeId);
            return View(model);
        }

        // GET: Models/Delete/{modelId}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var model = await _context.Models
                    .Include(m => m.Make)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Models/Delete/{modelId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var model = await _context.Models.FindAsync(id);
            var engines = this._context.Engines.Where(x => x.ModelId == model.Id);

            foreach (var engine in engines.ToList())
            {
                this._context.Engines.Remove(engine);
            }

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelExists(Guid id)
        {
            return _context.Models.Any(e => e.Id == id);
        }
    }
}
