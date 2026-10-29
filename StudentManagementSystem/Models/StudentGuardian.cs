using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class StudentGuardian
    {

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int GuardianId { get; set; }
        public Guardian Guardian { get; set; } = null!;


        [Required, MaxLength(40)]
        public string Relationship { get; set; } = string.Empty;

        public bool IsPrimaryContact { get; set; }

        public bool IsEmergencyContact { get; set; }

        public bool CanCollectStudent  { get; set; }
    }
}