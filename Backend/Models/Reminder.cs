using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotesApp_Postgre.Models
{
    public class Reminder
    {
        [Key]
        public int ReminderId { get; set; }

        public int NoteId { get; set; }

        [JsonIgnore]
        public virtual Note Note { get; set; } = null!;

        public string? ReminderText { get; set; } 

        [Required]
        public DateTime ReminderDate { get; set; }


    }
}
