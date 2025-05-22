using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Tutorial5.Data;
using Tutorial5.DTOs;

namespace Tutorial5.Services;

using Microsoft.EntityFrameworkCore;
using Tutorial5.DTOs;
using Tutorial5.Models;
using Tutorial5.Data;
using Tutorial5.Controllers;

public class DbService : IDbService
{
    private readonly PharmacyContext _context;

    public DbService(PharmacyContext context)
    {
        _context = context;
    }

    public async Task<int> AddPrescriptionAsync(AddPrescriptionRequestDto request)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == request.Doctor.IdDoctor);
        if (doctor == null)
        {
            throw new ArgumentException("Doktor nie istnieje.");
        }

        if (request.Prescription.DueDate < request.Prescription.Date)
        {
            throw new ArgumentException("Daty sa niezgodne.");
        }

        foreach (var medicamentDto in request.Medicaments)
        {
            var medicament = await _context.Medicaments.FirstOrDefaultAsync(m => m.IdMedicament == medicamentDto.IdMedicament);
            if (medicament == null)
            {
                throw new ArgumentException($"Lek z ID {medicamentDto.IdMedicament} nie istnieje.");
            }
        }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            Patient patient;
            if (request.Patient.IdPatient.HasValue && request.Patient.IdPatient.Value > 0)
            {
                patient = await _context.Patients.FirstOrDefaultAsync(p => p.IdPatient == request.Patient.IdPatient.Value);
                if (patient == null)
                {
                    patient = new Patient
                    {
                        FirstName = request.Patient.FirstName,
                        LastName = request.Patient.LastName,
                        Birthdate = request.Patient.Birthdate
                    };
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync(); 
                }
            }
            else
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthdate = request.Patient.Birthdate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync(); 
            }

            var prescription = new Prescription
            {
                Date = request.Prescription.Date,
                DueDate = request.Prescription.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor
            };
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync(); 


            foreach (var medicamentDto in request.Medicaments)
            {
                var prescriptionMedicament = new PrescriptionMedicament
                {
                    IdMedicament = medicamentDto.IdMedicament,
                    IdPrescription = prescription.IdPrescription,
                    Dose = medicamentDto.Dose ?? 0, 
                    Details = medicamentDto.Details
                };
                _context.PrescriptionMedicaments.Add(prescriptionMedicament);
            }
            await _context.SaveChangesAsync();

            scope.Complete();
            return prescription.IdPrescription;
        }
    }

    public async Task<GetPatientDetailsResponseDto> GetPatientDetailsAsync(int idPatient)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .Where(p => p.IdPatient == idPatient)
            .FirstOrDefaultAsync();

        if (patient == null)
        {
            return null;
        }

        var response = new GetPatientDetailsResponseDto
        {
            Patient = new PatientDetailsDto
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Birthdate = patient.Birthdate
            },
            Prescriptions = patient.Prescriptions
                .OrderBy(p => p.DueDate) 
                .Select(p => new PrescriptionDetailsDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorDetailsDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName,
                        Email = p.Doctor.Email
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentInPrescriptionDetailsDto
                    {
                        IdMedicament = pm.Medicament.IdMedicament,
                        Name = pm.Medicament.Name,
                        Description = pm.Medicament.Description,
                        Type = pm.Medicament.Type,
                        Dose = pm.Dose,
                        Details = pm.Details
                    }).ToList()
                }).ToList()
        };

        return response;
    }

    public async Task<bool> MedicamentExistsAsync(int idMedicament)
    {
        return await _context.Medicaments.AnyAsync(m => m.IdMedicament == idMedicament);
    }

    public async Task<Patient> GetPatientByIdAsync(int idPatient)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.IdPatient == idPatient);
    }

    public async Task<Doctor> GetDoctorByIdAsync(int idDoctor)
    {
        return await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == idDoctor);
    }
}
