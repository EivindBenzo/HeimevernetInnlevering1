using System.ComponentModel.DataAnnotations;

namespace Kart.Models
{
    public class RessursViewModel
    {

        [Required(ErrorMessage = "Navn må fylles ut")]
        public string Navn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type må fylles ut")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beskrivelse må fylles ut")]
        public string Beskrivelse { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
