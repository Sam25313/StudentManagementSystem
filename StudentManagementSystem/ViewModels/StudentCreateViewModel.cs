using Microsoft.AspNetCore.Http;
using StudentManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using static StudentManagementSystem.Models.SchoolEnums;

namespace StudentManagementSystem.ViewModels;

public class StudentCreateViewModel
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public Gender? Gender { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime? AdmissionDate { get; set; }

    [Required]
    public StudentType? StudentType { get; set; }

    public IFormFile? Photo { get; set; }
}