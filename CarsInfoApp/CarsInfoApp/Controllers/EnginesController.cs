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

    public class EnginesController : BaseController
    {
        // GET: Engines
        public async Task<IActionResult> Index(Guid id, string sortOrder, string searchString)
        {
            ViewBag.EngineCodeSortParm = String.IsNullOrEmpty(sortOrder) ? "engineCode_desc" : "";
            ViewBag.HpSortParm = sortOrder == "hp" ? "hp_desc" : "hp";
            var enginesQuery = from e in this._context.Engines
                               select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                enginesQuery = enginesQuery.Where(m => m.EngineCode.Contains(searchString) || m.Fuel.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "engineCode_desc":
                    enginesQuery = enginesQuery.OrderByDescending(m => m.EngineCode);
                    break;
                case "hp":
                    enginesQuery = enginesQuery.OrderBy(m => m.HorsePower);
                    break;
                case "hp_desc":
                    enginesQuery = enginesQuery.OrderByDescending(m => m.HorsePower);
                    break;
                default:
                    enginesQuery = enginesQuery.OrderBy(m => m.EngineCode);
                    break;
            }

            var model = this._context.Models.FirstOrDefault(x => x.Id == id);
            var engines = enginesQuery.Where(x => x.ModelId == id).ToList();
            foreach (var engine in engines)
            {
                engine.Model = model;
            }
            return View(engines);
        }

        // GET: Engines/Details/{engineId}
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var engine = await _context.Engines
                    .Include(e => e.Model)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (engine == null)
                {
                    return NotFound();
                }

                return View(engine);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Engines/Create
        public IActionResult Create(Guid id)
        {
            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                ViewData["ModelId"] = new SelectList(_context.Models, "Id", "ModelName");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Engines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelId,EngineCode,HorsePower,Torque,Fuel,HasTurbo")] Engine engine)
        {
            if (ModelState.IsValid)
            {
                engine.Id = Guid.NewGuid();
                _context.Add(engine);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Models");
            }
            ViewData["ModelId"] = new SelectList(_context.Models, "Id", "ModelName", engine.ModelId);
            return View(engine);
        }

        // GET: Engines/Edit/{engineId}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var engine = await _context.Engines.FindAsync(id);
                if (engine == null)
                {
                    return NotFound();
                }
                ViewData["ModelId"] = new SelectList(_context.Models, "Id", "ModelName", engine.ModelId);
                return View(engine);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Engines/Edit/{engineId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ModelId,EngineCode,HorsePower,Torque,Fuel,HasTurbo")] Engine engine)
        {
            if (id != engine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(engine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EngineExists(engine.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Models");
            }
            ViewData["ModelId"] = new SelectList(_context.Models, "Id", "ModelName", engine.ModelId);
            return View(engine);
        }

        // GET: Engines/Delete/{engineId}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isAuthorized = this.IsAuthorized();
            if (isAuthorized == Auth.Authorized)
            {
                var engine = await _context.Engines
                    .Include(e => e.Model)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (engine == null)
                {
                    return NotFound();
                }

                return View(engine);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Engines/Delete/{engineId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var engine = await _context.Engines.FindAsync(id);
            _context.Engines.Remove(engine);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Models");
        }

        private bool EngineExists(Guid id)
        {
            return _context.Engines.Any(e => e.Id == id);
        }
    }
}
