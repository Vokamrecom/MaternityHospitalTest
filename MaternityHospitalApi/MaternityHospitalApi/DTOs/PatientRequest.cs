using System.ComponentModel.DataAnnotations;

namespace MaternityHospitalApi.DTOs
{
    public class PatientRequest
    {
        [Required]
        public string Family { get; set; } = string.Empty;

        [Required]
        public ICollection<string> Given { get; set; } = new List<string>();
        [Required]
        public string Gender { get; set; } = "unknown";

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public bool Active { get; set; } = true;
    }
}
