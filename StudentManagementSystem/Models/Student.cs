//susing Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using static StudentManagementSystem.Models.SchoolEnums;

namespace StudentManagementSystem.Models
{
    public class Student
    {

        public int StudentId { get; set; }


        [System.ComponentModel.DataAnnotations.Required, MaxLength(30)]
        public string AdmissionNumber { get; set; } = string.Empty;


        [System.ComponentModel.DataAnnotations.Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public DateTime AdmissionDate  { get; set; }

        public StudentType StudentType { get; set; }

        public StudentStatus Status { get; set; } = StudentStatus.Active;

        [MaxLength(300)]
        public string? PhotoPath { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }


        public ICollection<StudentGuardian> Guardians { get; set; }
        = new List<StudentGuardian>();
    }
}
