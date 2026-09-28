using System.ComponentModel.DataAnnotations;

namespace HeimevernetInnlevering1.Models;

public class FormSubmission
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}

public class MapSubmission
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string LocationName { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
