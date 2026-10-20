using System;
using System.ComponentModel.DataAnnotations;

namespace YourProjectName.Models
{
    public class Note
    {
        [Key]
        public long NoteId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}