using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;

namespace library.Pages.Notes
{
    public class NoteItem
    {
        public long NoteId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string CreatedOn { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<NoteItem> Notes { get; set; } = new List<NoteItem>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string connectionString = _configuration.GetConnectionString("Library");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT note_id, title, body, created_on FROM lending.note ORDER BY note_id ASC;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Notes.Add(new NoteItem
                            {
                                NoteId = reader.GetInt64(0),
                                Title = reader.GetString(1),
                                Body = reader.IsDBNull(2) ? null : reader.GetString(2),
                                CreatedOn = reader.GetDateTime(3).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }
            }
        }
    }
}