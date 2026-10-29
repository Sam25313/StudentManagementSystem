using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers;

[Authorize(Policy = "ManageGuardians")]
public class GuardiansController : Controller
{
    private readonly ApplicationDbContext _db;

    public GuardiansController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.Guardians.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(g =>
                g.FullName.Contains(term) ||
                g.PhoneNumber.Contains(term));
        }

        var guardians = await query
            .OrderBy(g => g.FullName)
            .Take(200)
            .ToListAsync();

        return View(guardians);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new GuardianCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        GuardianCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var guardian = new Guardian
        {
            FullName = model.FullName.Trim(),
            PhoneNumber = model.PhoneNumber.Trim(),
            AlternativePhone = model.AlternativePhone?.Trim(),
            Email = model.Email?.Trim(),
            Address = model.Address?.Trim()
        };

        _db.Guardians.Add(guardian);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Guardian registered successfully.";

        return RedirectToAction(nameof(Index));
    }
}
