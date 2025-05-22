using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutorial5.Data;
using Tutorial5.Services;
using Tutorial5.DTOs;

namespace Tutorial5.Controllers;


[Route("api/[controller]")]
[ApiController]
public class PharmacyControler : ControllerBase
{
    private readonly IDbService _dbService;

    public PharmacyControler(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost("prescriptions")]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.Medicaments.Count > 10)
        {
            return BadRequest("Recepta moze zawierac maksymalnie 10 lekow.");
        }

        try
        {
            var newPrescriptionId = await _dbService.AddPrescriptionAsync(request);
            return CreatedAtAction(nameof(GetPatientDetails), new { idPatient = request.Patient.IdPatient ?? newPrescriptionId }, newPrescriptionId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Blad.");
        }
    }

    [HttpGet("patients/{idPatient}")]
    public async Task<IActionResult> GetPatientDetails(int idPatient)
    {
        var patientDetails = await _dbService.GetPatientDetailsAsync(idPatient);
        if (patientDetails == null)
        {
            return NotFound($"Pacjent z ID {idPatient} nie istnieje.");
        }
        return Ok(patientDetails);
    }
}
