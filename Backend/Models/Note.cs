using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotesApp_Postgre.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual Reminder? Reminder { get; set; }


        public DateTime? ReminderDate { get; set; }

        [JsonIgnore]
        public List<Tag> Tags { get; set; } = new List<Tag>();

        public virtual ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();


    }


    public class NoteDto
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
