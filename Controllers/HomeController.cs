using Microsoft.AspNetCore.Mvc;

namespace HeimevernetInnlevering1.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Form()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Form(FormSubmissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: When database is integrated (from Marius branch), add:
        // _context.FormSubmissions.Add(new FormSubmission { ... });
        // await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Result), new
        {
            name = $"{model.FirstName} {model.LastName}",
            email = model.Email,
            message = model.Message
        });
    }

    [HttpGet]
    public IActionResult Result(string name, string email, string message)
    {
        ViewBag.Name = name;
        ViewBag.Email = email;
        ViewBag.Message = message;
        return View();
    }

    [HttpGet]
    public IActionResult Map()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Map(MapSubmissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: When database is integrated (from Marius branch), add:
        // _context.MapSubmissions.Add(new MapSubmission { ... });
        // await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MapResult), new
        {
            locationName = model.LocationName,
            latitude = model.Latitude,
            longitude = model.Longitude
        });
    }

    [HttpGet]
    public IActionResult MapResult(string locationName, double latitude, double longitude)
    {
        ViewBag.LocationName = locationName;
        ViewBag.Latitude = latitude;
        ViewBag.Longitude = longitude;
        return View();
    }

    [HttpGet]
    public IActionResult Data()
    {
        // TODO: When database is integrated (from Marius branch), add:
        // var formSubmissions = await _context.FormSubmissions.ToListAsync();
        // var mapSubmissions = await _context.MapSubmissions.ToListAsync();
        // return View(new DataViewModel { FormSubmissions = formSubmissions, MapSubmissions = mapSubmissions });

        return View();
    }
}
