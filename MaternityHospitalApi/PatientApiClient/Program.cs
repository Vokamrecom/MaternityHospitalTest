using System.Text.Json;
using System.Text;

namespace PatientApiClient
{
    class Program
    {
        private static readonly HttpClient _client = new HttpClient();

        static async Task Main(string[] args)
        {
            string apiUrl = "http://maternityhospitalapi:5000/api/patients";
            //string apiUrl = "https://localhost:44350/api/Patients";

            for (int i = 1; i <= 100; i++)
            {
                var patient = new
                {
                    family = $"Family_{i}",
                    given = new List<string> { $"Given_{i}_1", $"Given_{i}_2" },
                    gender = i % 2 == 0 ? "male" : "female",
                    birthDate = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd"),
                    active = i % 3 != 0
                };

                var json = JsonSerializer.Serialize(patient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Patient {i} added successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to add Patient {i}: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding Patient {i}: {ex.Message}");
                }
            }

            Console.WriteLine("Finished adding patients.");
        }
    }
}