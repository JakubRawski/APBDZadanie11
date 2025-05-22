using Microsoft.EntityFrameworkCore;
using Tutorial5.Models;

namespace Tutorial5.Data;

public class PharmacyContext : DbContext
    {
        public PharmacyContext(DbContextOptions<PharmacyContext> options) : base(options)
        {
        }

        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define composite primary key for PrescriptionMedicament
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

            // Relationships
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdMedicament);

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.IdPatient);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.IdDoctor);

            // Seed data for initial testing (optional, but good for quick setup)
            modelBuilder.Entity<Medicament>().HasData(
                new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Pain reliever", Type = "Analgesic" },
                new Medicament { IdMedicament = 2, Name = "Amoxicillin", Description = "Antibiotic", Type = "Antibiotic" },
                new Medicament { IdMedicament = 3, Name = "Ibuprofen", Description = "Anti-inflammatory", Type = "NSAID" }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { IdPatient = 1, FirstName = "Jan", LastName = "Kowalski", Birthdate = new DateTime(1990, 5, 15) },
                new Patient { IdPatient = 2, FirstName = "Anna", LastName = "Nowak", Birthdate = new DateTime(1985, 10, 20) }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { IdDoctor = 1, FirstName = "Adam", LastName = "Mickiewicz", Email = "adam.mickiewicz@example.com" },
                new Doctor { IdDoctor = 2, FirstName = "Maria", LastName = "Sklodowska", Email = "maria.sklodowska@example.com" }
            );
        }
    }