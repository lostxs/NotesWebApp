using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Models;
using NotesApp_Postgre.Service;

namespace NotesApp_Postgre.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }

        public static implicit operator AppDbContext(ReminderService v)
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определение отношения многие ко многим между заметками и тегами
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags)
                .WithMany(t => t.Notes)
                .UsingEntity(j => j.ToTable("NoteTag"));
        }
    }
}