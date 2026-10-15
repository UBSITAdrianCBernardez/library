using Microsoft.AspNetCore.Mvc;
using library.Data;
using library.Models;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _repository;

    public MembersController(MemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _repository.GetAllAsync();
        return View(members);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(member);
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member == null) return NotFound();
        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Member member)
    {
        if (id != member.MemberId) return NotFound();

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(member);
            return RedirectToAction(nameof(Index));
        }
        return View(member);
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
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}