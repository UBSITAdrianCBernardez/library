using Npgsql;
using Microsoft.AspNetCore.Mvc;
using library.Data;
using library.Models;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _repository;
    private readonly LoanRepository _loans;

    public MembersController(MemberRepository repository, LoanRepository loans)
    {
        _repository = repository;
        _loans = loans;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _repository.GetAllAsync();
        return View("~/Views/Members/Index.cshtml", members);
    }

    public async Task<IActionResult> Details(long id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }

        var loans = await _loans.GetByMemberAsync(id);

        var viewModel = new MemberDetails
        {
            Member = member,
            Loans = loans
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _repository.AddAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null) return NotFound();
        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _repository.UpdateAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null) return NotFound();
        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var member = await _repository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Error = "This member has loans on record, so the database will not delete them.";
            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }
}