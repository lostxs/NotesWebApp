namespace NotesApp_Postgre.Models
{
    public class ReminderRequestModel
    {
        public int NoteId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? ReminderDate { get; set; }
    }
}
