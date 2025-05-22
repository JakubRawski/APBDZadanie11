using Tutorial5.DTOs;
using Tutorial5.Models;

namespace Tutorial5.Services;

public interface IDbService
{
    Task<int> AddPrescriptionAsync(AddPrescriptionRequestDto request);
    Task<GetPatientDetailsResponseDto> GetPatientDetailsAsync(int idPatient);
    Task<bool> MedicamentExistsAsync(int idMedicament);
    Task<Patient> GetPatientByIdAsync(int idPatient);
    Task<Doctor> GetDoctorByIdAsync(int idDoctor);
}