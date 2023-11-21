using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp_Postgre.Models
{
    public class NoteTag
    {


        public int NoteID { get; set; }

        public int TagID { get; set; }

        public virtual Note Note { get; set; } = new Note();
        public virtual Tag Tag { get; set; } = new Tag();

        public int NoteTagId { get; set; }
    }
    public class NoteTagDto
    {
        public int NoteID { get; set; }
        public int TagID { get; set; }
    }
}