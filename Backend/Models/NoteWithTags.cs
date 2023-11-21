namespace NotesApp_Postgre.Models
{
    public class NoteWithTags
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ReminderDate { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
