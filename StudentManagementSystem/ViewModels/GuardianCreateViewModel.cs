using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels;

public class GuardianCreateViewModel
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Phone, MaxLength(20)]
    public string? AlternativePhone { get; set; }

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }
}
