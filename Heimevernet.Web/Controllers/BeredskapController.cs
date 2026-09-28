using Microsoft.AspNetCore.Mvc;
using Heimevernet.Web.Models;

namespace Heimevernet.Web.Controllers;

/// <summary>
/// Håndterer innmelding av hendelser for Heimevernet.
/// </summary>
public class BeredskapController : Controller
{
    /// <summary>
    /// Viser skjema med kart for ny hendelsesmelding (GET).
    /// </summary>
    [HttpGet]
    public IActionResult MeldHendelse()
    {
        return View(CreateDefaultModel());
    }

    /// <summary>
    /// Mottar og validerer hendelsesmelding fra skjema (POST).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MeldHendelse(IncidentReportFormViewModel model)
    {
        model.AvailableIncidentTypes = GetIncidentTypes();
        model.ServerGeneratedAt = DateTime.UtcNow;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = new IncidentReportResultViewModel
        {
            SubmittedReport = model,
            ProcessedAt = DateTime.UtcNow
        };

        return View("MeldingMottatt", result);
    }

    private static IncidentReportFormViewModel CreateDefaultModel()
    {
        return new IncidentReportFormViewModel
        {
            IncidentTime = DateTime.UtcNow,
            ServerGeneratedAt = DateTime.UtcNow,
            AvailableIncidentTypes = GetIncidentTypes()
        };
    }

    private static IReadOnlyList<string> GetIncidentTypes()
    {
        return [
            "Observasjon",
            "Sambandsutfall",
            "Skade på infrastruktur",
            "Behov for støtte",
            "Annet"
        ];
    }
}
