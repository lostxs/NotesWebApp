using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Context;
using NotesApp_Postgre.Models;
using NotesApp_Postgre.Service;

namespace NotesApp_Postgre.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class RemindersController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ReminderService _reminderService;

        public RemindersController(ReminderService reminderService, AppDbContext context)

        {
            _reminderService = reminderService;
            _context = context;
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendReminders([FromQuery] int noteId)
        {
            var note = await _context.Notes
                .Include(n => n.Reminder)
                .FirstOrDefaultAsync(n => n.NoteId == noteId);

            if (note == null || note.Reminder == null)
            {
                return NotFound("Заметка или напоминание не найдены");
            }

            var message = $"Напоминание для заметки \"{note.Title}\": {note.Reminder.ReminderText}";

            await _reminderService.SendReminders(noteId, message);

            return Ok("Уведомление отправлено успешно.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReminders()
        {
            var reminders = await _reminderService.GetAllReminders();
            return Ok(reminders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminder([FromBody] Reminder model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

              
                var existingNote = await _context.Notes.FindAsync(model.NoteId);

                if (existingNote == null)
                {
                    return BadRequest($"The associated note with NoteId {model.NoteId} does not exist.");
                }

               
                var newReminder = new Reminder
                {
                    NoteId = model.NoteId,
                    ReminderText = model.ReminderText,
                    ReminderDate = model.ReminderDate
                };


                _context.Reminders.Add(newReminder);

             
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReminderById), new { id = newReminder.ReminderId }, newReminder);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminderById(int id)
        {
          
            var reminder = await _context.Reminders.FindAsync(id);

            if (reminder == null)
            {
                return NotFound();
            }

            return Ok(reminder);
        }
    }
}