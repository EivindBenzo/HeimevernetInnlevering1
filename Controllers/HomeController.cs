using System.Globalization;
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

        var lat = HttpContext.Session.GetString("PendingLat");
        var lng = HttpContext.Session.GetString("PendingLng");

        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        {
            model.Latitude = double.Parse(lat, CultureInfo.InvariantCulture);
            model.Longitude = double.Parse(lng, CultureInfo.InvariantCulture);
            model.LocationName = HttpContext.Session.GetString("PendingLocationName");
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
    public async Task<IActionResult> Map()
    {
        await LoadResourcesAsync();
        return View(new MapSubmissionViewModel
        {
            LocationName = "Kristiansand",
            Latitude = 58.1467,
            Longitude = 7.9956
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Map(MapSubmissionViewModel model)
    {
        if (!MapSubmissionViewModel.ResourceTypes.Contains(model.ResourceType))
        {
            ModelState.AddModelError(nameof(model.ResourceType), "Velg en gyldig ressurstype");
        }

        if (!ModelState.IsValid)
        {
            await LoadResourcesAsync();
            return View(model);
        }

        _db.MapSubmissions.Add(new MapSubmission
        {
            ResourceName = model.ResourceName,
            ResourceType = model.ResourceType,
            LocationName = model.LocationName,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        HttpContext.Session.SetString("PendingLat", model.Latitude.ToString(CultureInfo.InvariantCulture));
        HttpContext.Session.SetString("PendingLng", model.Longitude.ToString(CultureInfo.InvariantCulture));
        HttpContext.Session.SetString("PendingLocationName", model.LocationName);

        TempData["MapSuccess"] = $"{model.ResourceType} «{model.ResourceName}» ble registrert ved {model.LocationName}.";

        // Back to the map so the new marker is shown together with the others.
        return RedirectToAction(nameof(Map));
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
    public async Task<IActionResult> Data() => View(await BuildDataModelAsync());

    [HttpGet]
    public async Task<IActionResult> Verify() => View(await BuildDataModelAsync());

    private async Task LoadResourcesAsync()
    {
        ViewBag.Resources = await _db.MapSubmissions
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    private async Task<DataPageViewModel> BuildDataModelAsync() => new()
    {
        FormSubmissions = await _db.FormSubmissions.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(),
        MapSubmissions = await _db.MapSubmissions.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync()
    };
}
