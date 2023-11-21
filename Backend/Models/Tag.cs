using System.ComponentModel.DataAnnotations;

namespace NotesApp_Postgre.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<Note> Notes { get; set; } = new List<Note>();

        public virtual ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>(); 
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public List<NoteDto> Notes { get; set; }
    }
}
