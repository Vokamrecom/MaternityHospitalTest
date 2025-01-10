namespace MaternityHospitalApi.DTOs
{
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public string Family { get; set; } = string.Empty;
        public ICollection<string> Given { get; set; } = new List<string>();
        public string Gender { get; set; } = "unknown";
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; } = true;
    }
}
