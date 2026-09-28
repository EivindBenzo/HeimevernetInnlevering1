namespace Heimevernet.Web.Models;

/// <summary>
/// Representerer bekreftet innsending av hendelsesmelding.
/// </summary>
public class IncidentReportResultViewModel
{
    /// <summary>
    /// Mottatt innsending.
    /// </summary>
    public IncidentReportFormViewModel SubmittedReport { get; set; } = new();

    /// <summary>
    /// Tidspunkt når meldingen ble behandlet av server.
    /// </summary>
    public DateTime ProcessedAt { get; set; }
}
