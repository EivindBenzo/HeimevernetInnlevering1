using Microsoft.AspNetCore.Mvc;
using Kart.Models;
using Kart.Data;
using Microsoft.EntityFrameworkCore;

namespace Kart.Controllers
{
    public class Controllerkart : Controller
    {
        private readonly AppDbContext _context;

        public Controllerkart(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var viewModel = new ControllerkartViewModel
            {
                RegistrerteRessurser = await _context.Ressurser.ToListAsync()
            };

            return View(viewModel);
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> Index(ControllerkartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegistrerteRessurser = await _context.Ressurser.ToListAsync();

                return View(model);
            }

            var nyRessurs = new Ressurs
            {
                Navn = model.NyRessurs.Navn,
                Type = model.NyRessurs.Type,
                Beskrivelse = model.NyRessurs.Beskrivelse,
                Latitude = model.NyRessurs.Latitude,
                Longitude = model.NyRessurs.Longitude
            };

            _context.Ressurser.Add(nyRessurs);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}