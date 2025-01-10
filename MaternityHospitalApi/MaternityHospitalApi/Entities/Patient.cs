using System.ComponentModel.DataAnnotations;

namespace MaternityHospitalApi.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        public string Family { get; set; } = string.Empty;
        public IList<GivenName> Given { get; set; } = new List<GivenName>();
        public string Gender { get; set; } = "unknown"; // male, female, other, unknown
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; } = true;
    }
}
