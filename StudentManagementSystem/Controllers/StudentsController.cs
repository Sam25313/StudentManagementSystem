using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.ViewModels;
using static StudentManagementSystem.Models.SchoolEnums;

namespace StudentManagementSystem.Controllers;

[Authorize(Policy = "ManageStudents")]
public class StudentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly StudentRegistrationService _registration;
    private readonly StudentPhotoService _photos;

    public StudentsController(
        ApplicationDbContext db,
        StudentRegistrationService registration,
        StudentPhotoService photos)
    {
        _db = db;
        _registration = registration;
        _photos = photos;
    }

    public async Task<IActionResult> Index(
        string? search,
        StudentStatus? status,
        StudentType? studentType)
    {
        var query = _db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(s =>
                s.AdmissionNumber.Contains(term) ||
                s.FirstName.Contains(term) ||
                s.LastName.Contains(term));
        }

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (studentType.HasValue)
            query = query.Where(s => s.StudentType == studentType.Value);

        var students = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Take(200)
            .ToListAsync();

        return View(students);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new StudentCreateViewModel
        {
            AdmissionDate = DateTime.Today,
            DateOfBirth = DateTime.Today.AddYears(-6)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        StudentCreateViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.DateOfBirth.HasValue &&
            model.DateOfBirth.Value.Date >= DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(model.DateOfBirth),
                "Date of birth must be in the past.");
        }

        if (model.AdmissionDate.HasValue &&
            model.AdmissionDate.Value.Date > DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(model.AdmissionDate),
                "Admission date cannot be in the future.");
        }

        if (!ModelState.IsValid)
            return View(model);

        string? photoPath = null;

        try
        {
            photoPath = await _photos.SaveAsync(
                model.Photo, cancellationToken);

            var student = new Student
            {
                FirstName = model.FirstName.Trim(),
                MiddleName = model.MiddleName?.Trim(),
                LastName = model.LastName.Trim(),
                DateOfBirth = model.DateOfBirth!.Value.Date,
                Gender = model.Gender!.Value,
                AdmissionDate = model.AdmissionDate!.Value.Date,
                StudentType = model.StudentType!.Value,
                Status = StudentStatus.Active,
                PhotoPath = photoPath
            };

            await _registration.RegisterAsync(
                student, cancellationToken);

            TempData["Success"] =
                $"Student registered. Admission number: {student.AdmissionNumber}";

            return RedirectToAction(nameof(Details),
                new { id = student.StudentId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Photo), ex.Message);
        }

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.Guardians)
                .ThenInclude(sg => sg.Guardian)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        student.Status = StudentStatus.Withdrawn;
        student.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Student marked as withdrawn.";

        return RedirectToAction(nameof(Details), new { id });
    }

    //This is to Link a Guardian to a Student

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkGuardian(
    int id,
    int guardianId,
    string relationship,
    bool isPrimaryContact,
    bool isEmergencyContact,
    bool canCollectStudent)
    {
        if (string.IsNullOrWhiteSpace(relationship) ||
            relationship.Length > 40)
        {
            ModelState.AddModelError(
                nameof(relationship), "Enter a valid relationship.");

            return BadRequest(ModelState);
        }

        var studentExists = await _db.Students
            .AnyAsync(s => s.StudentId == id);

        var guardianExists = await _db.Guardians
            .AnyAsync(g => g.GuardianId == guardianId);

        if (!studentExists || !guardianExists)
            return NotFound();

        var alreadyLinked = await _db.StudentGuardians
            .AnyAsync(sg =>
                sg.StudentId == id &&
                sg.GuardianId == guardianId);

        if (alreadyLinked)
        {
            TempData["Error"] = "This guardian is already linked.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        if (isPrimaryContact)
        {
            var existingPrimary = await _db.StudentGuardians
                .Where(sg => sg.StudentId == id && sg.IsPrimaryContact)
                .ToListAsync();

            foreach (var existing in existingPrimary)
                existing.IsPrimaryContact = false;
        }

        _db.StudentGuardians.Add(new StudentGuardian
        {
            StudentId = id,
            GuardianId = guardianId,
            Relationship = relationship.Trim(),
            IsPrimaryContact = isPrimaryContact,
            IsEmergencyContact = isEmergencyContact,
            CanCollectStudent = canCollectStudent
        });

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["Success"] = "Guardian linked successfully.";

        return RedirectToAction(nameof(Details), new { id });
    }
}
