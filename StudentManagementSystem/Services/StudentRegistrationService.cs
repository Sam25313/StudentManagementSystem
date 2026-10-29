
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services;

public class StudentRegistrationService
{
    private readonly ApplicationDbContext _db;

    public StudentRegistrationService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Student> RegisterAsync(
        Student student,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync(
                cancellationToken);

        student.AdmissionNumber = $"TMP - {Guid.NewGuid()}"[..24];  

        _db.Students.Add(student);

        await _db.SaveChangesAsync(cancellationToken);

        student.AdmissionNumber =
            $"SCH/{student.AdmissionDate.Year}/{student.StudentId:D5}";

        await _db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return student;
    }
}
