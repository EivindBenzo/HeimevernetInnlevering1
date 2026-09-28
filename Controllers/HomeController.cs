using HeimevernetInnlevering1.Data;
using HeimevernetInnlevering1.Models;
using HeimevernetInnlevering1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeimevernetInnlevering1.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Form()
    {
        var model = new FormSubmissionViewModel();

        // Pre-fill with a location chosen on the Kart page in this session, if any.
        var lat = HttpContext.Session.GetString("PendingLat");
        var lng = HttpContext.Session.GetString("PendingLng");
        var locationName = HttpContext.Session.GetString("PendingLocationName");

        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        {
            model.Latitude = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
            model.Longitude = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
            model.LocationName = locationName;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Form(FormSubmissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var submission = new FormSubmission
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Description = model.Message,
            LocationName = model.LocationName,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.FormSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        HttpContext.Session.SetInt32("RegistrationId", submission.Id);
        HttpContext.Session.Remove("PendingLat");
        HttpContext.Session.Remove("PendingLng");
        HttpContext.Session.Remove("PendingLocationName");

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
    public IActionResult Map() => View(new MapSubmissionViewModel
    {
        LocationName = "Kristiansand",
        Latitude = 58.1467,
        Longitude = 7.9956
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Map(MapSubmissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var submission = new MapSubmission
        {
            LocationName = model.LocationName,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.MapSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        // Remember this location so it can be linked to the user's Skjema registration.
        HttpContext.Session.SetString("PendingLat", model.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
        HttpContext.Session.SetString("PendingLng", model.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
        HttpContext.Session.SetString("PendingLocationName", model.LocationName);

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
    public async Task<IActionResult> Data()
    {
        var model = new DataPageViewModel
        {
            FormSubmissions = await _db.FormSubmissions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(),
            MapSubmissions = await _db.MapSubmissions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync()
        };

        return View(model);
    }

    // Simple verification page: proves data survives app restarts by reading
    // straight from the database and showing row counts and raw rows.
    [HttpGet]
    public async Task<IActionResult> Verify()
    {
        var model = new DataPageViewModel
        {
            FormSubmissions = await _db.FormSubmissions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(),
            MapSubmissions = await _db.MapSubmissions
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync()
        };

        return View(model);
    }
}
