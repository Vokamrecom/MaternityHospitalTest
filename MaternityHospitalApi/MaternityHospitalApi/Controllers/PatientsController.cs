using MaternityHospitalApi.DTOs;
using MaternityHospitalApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace MaternityHospitalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly DataContext _context;

        public PatientsController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new patient", Description = "Adds a new patient to the database.")]
        [SwaggerResponse(201, "Patient created successfully.", typeof(PatientResponse))]
        [SwaggerResponse(400, "Invalid input.")]
        public async Task<ActionResult<PatientResponse>> CreatePatient(PatientRequest request)
        {
            if (string.IsNullOrEmpty(request.Family) || request.BirthDate == default)
                return BadRequest("Family and BirthDate are required fields.");

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                Family = request.Family,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                Active = request.Active,
                Given = request.Given.Select(x => new GivenName { Name = x }).ToList()
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var response = new PatientResponse
            {
                Id = patient.Id,
                Family = patient.Family,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Active = patient.Active,
                Given = patient.Given.Select(x => x.Name).ToList()
            };

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, response);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves a patient by ID", Description = "Gets the details of a specific patient using their unique ID.")]
        [SwaggerResponse(200, "Patient found.", typeof(PatientResponse))]
        [SwaggerResponse(404, "Patient not found.")]
        public async Task<ActionResult<PatientResponse>> GetPatient(Guid id)
        {
            var patient = await _context.Patients
                .Include(p => p.Given)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null) return NotFound();

            var response = new PatientResponse
            {
                Id = patient.Id,
                Family = patient.Family,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Active = patient.Active,
                Given = patient.Given.Select(x => x.Name).ToList()
            };

            return response;
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search patients by birthDate", Description = "Finds patients by birthDate based on search parameters.")]
        [SwaggerResponse(200, "Patients found.", typeof(List<PatientResponse>))]
        public async Task<ActionResult<List<PatientResponse>>> SearchByBirthDate([FromQuery] string birthDate)
        {
            if (string.IsNullOrEmpty(birthDate))
            {
                return BadRequest("birthDate is required");
            }

            var condition = birthDate.Substring(0, 2).ToLower();
            if (!DateTime.TryParse(birthDate.Substring(2), out var parsedDate))
            {
                return BadRequest("Invalid date format");
            }

            IQueryable<Patient> patients = _context.Patients.Include(p => p.Given);
            DateTime now = DateTime.Now;
            double toleranceDays = Math.Abs((now - parsedDate).TotalDays) * 0.10;

            List<Patient> query = new List<Patient>(
                condition switch
                {
                    "lt" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date < parsedDate.Date
                            : p.BirthDate < parsedDate),
                    "gt" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date > parsedDate.Date
                            : p.BirthDate > parsedDate),
                    "eq" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date == parsedDate.Date
                            : p.BirthDate == parsedDate),
                    "ne" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date != parsedDate.Date
                            : p.BirthDate != parsedDate),
                    "le" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date <= parsedDate.Date
                            : p.BirthDate <= parsedDate),
                    "sa" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date > parsedDate.Date
                            : p.BirthDate > parsedDate),
                    "eb" => patients.Where(p =>
                        parsedDate.TimeOfDay == TimeSpan.Zero
                            ? p.BirthDate.Date < parsedDate.Date
                            : p.BirthDate < parsedDate),
                    "ap" => patients.AsEnumerable().Where(p =>
                        Math.Abs((p.BirthDate - parsedDate).TotalDays) <= toleranceDays),
                    _ => throw new ArgumentException("Invalid condition")
                });

            return query.Select(patient => new PatientResponse
            {
                Id = patient.Id,
                Family = patient.Family,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Active = patient.Active,
                Given = patient.Given.Select(x => x.Name).ToList()
            }).ToList();
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates a patient", Description = "Updates the details of an existing patient.")]
        [SwaggerResponse(200, "Patient updated successfully.", typeof(PatientResponse))]
        [SwaggerResponse(400, "Invalid input.")]
        [SwaggerResponse(404, "Patient not found.")]
        public async Task<ActionResult<PatientResponse>> UpdatePatient(Guid id, PatientRequest request)
        {
            var patient = await _context.Patients
                .Include(x => x.Given)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null) return NotFound();

            patient.Family = request.Family;
            patient.Gender = request.Gender;
            patient.BirthDate = request.BirthDate;
            patient.Active = request.Active;

            patient.Given.Clear();
            patient.Given = request.Given.Select(x => new GivenName
            {
                Name = x,
                PatientId = patient.Id
            }).ToList();

            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var response = new PatientResponse
            {
                Id = patient.Id,
                Family = patient.Family,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Active = patient.Active,
                Given = patient.Given.Select(g => g.Name).ToList()
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a patient", Description = "Removes a patient from the database using their unique ID.")]
        [SwaggerResponse(200, "Patient deleted successfully.", typeof(PatientResponse))]
        [SwaggerResponse(404, "Patient not found.")]
        public async Task<ActionResult<PatientResponse>> DeletePatient(Guid id)
        {
            var patient = await _context.Patients
                .Include(x => x.Given)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null) return NotFound();

            var response = new PatientResponse
            {
                Id = patient.Id,
                Family = patient.Family,
                Gender = patient.Gender,
                BirthDate = patient.BirthDate,
                Active = patient.Active,
                Given = patient.Given.Select(x => x.Name).ToList()
            };

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Ok(response);
        }

        private bool PatientExists(Guid id) => _context.Patients.Any(e => e.Id == id);
    }
}