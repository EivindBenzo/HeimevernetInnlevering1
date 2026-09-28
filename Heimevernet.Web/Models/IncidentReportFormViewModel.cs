using System.ComponentModel.DataAnnotations;

namespace Heimevernet.Web.Models;

/// <summary>
/// Representerer brukerdata for innsending av hendelsesmelding.
/// </summary>
public class IncidentReportFormViewModel
{
    /// <summary>
    /// Navn på innsender.
    /// </summary>
    [Required(ErrorMessage = "Navn er påkrevd.")]
    [Display(Name = "Navn")]
    public string ReporterName { get; set; } = string.Empty;

    /// <summary>
    /// Navn på tropp eller avdeling.
    /// </summary>
    [Required(ErrorMessage = "Avdeling er påkrevd.")]
    [Display(Name = "Avdeling")]
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// Type hendelse.
    /// </summary>
    [Required(ErrorMessage = "Velg hendelsestype.")]
    [Display(Name = "Hendelsestype")]
    public string IncidentType { get; set; } = string.Empty;

    /// <summary>
    /// Beskrivelse av hendelsen.
    /// </summary>
    [Required(ErrorMessage = "Beskrivelse er påkrevd.")]
    [StringLength(500, ErrorMessage = "Beskrivelsen kan være maksimalt 500 tegn.")]
    [Display(Name = "Beskrivelse")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Tidspunkt for observasjonen.
    /// </summary>
    [Display(Name = "Observasjonstid")]
    public DateTime IncidentTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Breddegrad fra kart.
    /// </summary>
    [Required(ErrorMessage = "Klikk i kartet for å velge posisjon.")]
    [Range(-90, 90, ErrorMessage = "Breddegrad må være mellom -90 og 90.")]
    [Display(Name = "Breddegrad")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Lengdegrad fra kart.
    /// </summary>
    [Required(ErrorMessage = "Klikk i kartet for å velge posisjon.")]
    [Range(-180, 180, ErrorMessage = "Lengdegrad må være mellom -180 og 180.")]
    [Display(Name = "Lengdegrad")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Servergenerert tidspunkt brukt for dynamisk innhold.
    /// </summary>
    public DateTime ServerGeneratedAt { get; set; }

    /// <summary>
    /// Tilgjengelige hendelsestyper fra webserver.
    /// </summary>
    public IReadOnlyList<string> AvailableIncidentTypes { get; set; } = [];
}
