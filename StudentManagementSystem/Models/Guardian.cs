
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Guardian
    {

        public int GuardianId { get; set; }



        [Required, MaxLength(30)]
        public string FullName { get; set; } = string.Empty;


        [Required, MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string?  AlternativePhone{ get; set; }

        [ EmailAddress, MaxLength(300)]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }


        public ICollection<StudentGuardian> Students { get; set; } 

        = new List<StudentGuardian>();






    }
}
