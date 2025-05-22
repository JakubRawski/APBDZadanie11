namespace Tutorial5.DTOs;
using System.ComponentModel.DataAnnotations;

public class AddPrescriptionRequestDto
{
    [Required]
    public PrescriptionDto Prescription { get; set; }

    [Required]
    public PatientDto Patient { get; set; }

    [Required]
    public DoctorDto Doctor { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(10)]
    public List<MedicamentInPrescriptionDto> Medicaments { get; set; }
}

public class PrescriptionDto
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
}

public class PatientDto
{
    public int? IdPatient { get; set; } 
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public DateTime Birthdate { get; set; }
}

public class DoctorDto
{
    [Required]
    public int IdDoctor { get; set; } 
}

public class MedicamentInPrescriptionDto
{
    [Required]
    public int IdMedicament { get; set; }
    public int? Dose { get; set; }
    public string Details { get; set; }
}

public class GetPatientDetailsResponseDto
{
    public PatientDetailsDto Patient { get; set; }
    public List<PrescriptionDetailsDto> Prescriptions { get; set; }
}

public class PatientDetailsDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
}

public class PrescriptionDetailsDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentInPrescriptionDetailsDto> Medicaments { get; set; }
    public DoctorDetailsDto Doctor { get; set; }
}

public class MedicamentInPrescriptionDetailsDto
{
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int Dose { get; set; }
    public string Details { get; set; }
}

public class DoctorDetailsDto
{
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

