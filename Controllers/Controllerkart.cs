using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Kart.Models;
using System.Collections.Generic;

namespace Kart.Controllers
{
    public class Controllerkart : Controller
    {
        private static List<Ressurs> ressurser = new List<Ressurs>();
        public IActionResult Index()
        {
            return View(ressurser);
        }
        [HttpPost]
        public IActionResult Index(RessursViewModel ressurs)
        {
            var nyRessurs = new Ressurs
            {
                Navn = ressurs.Navn,
                Type = ressurs.Type,
                Beskrivelse = ressurs.Beskrivelse,
                Latitude = ressurs.Latitude,
                Longitude = ressurs.Longitude
            };

            ressurser.Add(nyRessurs);

            return View(ressurser);
        }
    }
 }

