using System.ComponentModel.DataAnnotations;

namespace library.Models;

public class BorrowForm
{
    [Required(ErrorMessage = "Please choose a member.")]
    public long? MemberId { get; set; }

    [Required(ErrorMessage = "Please choose a book.")]
    public long? BookId { get; set; }

    [Range(1, 90, ErrorMessage = "Days allowed must be between 1 and 90.")]
    public int DaysAllowed { get; set; } = 14;
}