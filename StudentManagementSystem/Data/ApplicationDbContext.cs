
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();

        public DbSet<Guardian> Guardians => Set<Guardian>();

        public DbSet<StudentGuardian> StudentGuardians
            => Set<StudentGuardian>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            // Keep the configuration required by ASP.NET Core Identity.
            base.OnModelCreating(modelBuilder);

            // Admission numbers must be unique.
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.AdmissionNumber)
                .IsUnique();

            // Store enum values as readable strings.
            modelBuilder.Entity<Student>()
                .Property(s => s.Gender)
                .HasConversion<string>();

            modelBuilder.Entity<Student>()
                .Property(s => s.StudentType)
                .HasConversion<string>();

            modelBuilder.Entity<Student>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // Each student-guardian pair must be unique.
            modelBuilder.Entity<StudentGuardian>()
                .HasKey(sg => new
                {
                    sg.StudentId,
                    sg.GuardianId
                });

            // Connect StudentGuardian to Student.
            modelBuilder.Entity<StudentGuardian>()
                .HasOne(sg => sg.Student)
                .WithMany(s => s.Guardians)
                .HasForeignKey(sg => sg.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Connect StudentGuardian to Guardian.
            modelBuilder.Entity<StudentGuardian>()
                .HasOne(sg => sg.Guardian)
                .WithMany(g => g.Students)
                .HasForeignKey(sg => sg.GuardianId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure that each student can have only one primary contact.
            modelBuilder.Entity<StudentGuardian>()
                .HasIndex(x => x.StudentId)
                .IsUnique()
                .HasFilter("[IsPrimaryContact] = 1");   

            // Improve common searches.
            modelBuilder.Entity<Student>()
                .HasIndex(s => new
                {
                    s.LastName,
                    s.FirstName
                });

            modelBuilder.Entity<Guardian>()
                .HasIndex(g => g.PhoneNumber);

            // Admission number is required and limited to 30 characters.
            modelBuilder.Entity<Student>()
                .Property(s => s.AdmissionNumber)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
