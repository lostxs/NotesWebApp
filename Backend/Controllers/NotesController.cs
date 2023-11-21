using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Context;
using NotesApp_Postgre.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using NotesApp_Postgre.Service;

namespace NotesApp_Postgre.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ReminderService _notificationService;

        public NotesController(AppDbContext context, ReminderService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }




        // Получение списка всех заметок
        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var notes = await _context.Notes
                .Include(n => n.Tags)
                .Include(n => n.Reminder) 
                .AsNoTracking()
                .ToListAsync();

            return Ok(notes);
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        [HttpGet("tags-with-notes")]
        public IActionResult GetTagsWithNotes()
        {
            var tagsWithNotes = _context.Tags
                .Include(tag => tag.Notes)
                .ToList();

            var tagDtos = tagsWithNotes.Select(tag => new TagDto
            {
                TagId = tag.TagId,
                Name = tag.Name,
                Notes = tag.Notes.Select(note => new NoteDto
                {
                    NoteId = note.NoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreationDate = note.CreationDate,
                   
                }).ToList()
            }).ToList();

            return Ok(tagDtos);
        }






        // Создание нового тега
        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);

                if (existingTag == null)
                {
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetTags), tag);
                }

                return Conflict($"Tag with name '{tag.Name}' already exists");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        // Удаление тега
        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(id);

                if (tag == null)
                {
                    return NotFound();
                }

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        [HttpGet("notes-with-reminders")]
        public IActionResult GetNotesWithReminders()
        {
            var notesWithReminders = _context.Notes
                .Include(n => n.Reminder)
                .Where(n => n.Reminder != null) 
                .ToList();

            return Ok(notesWithReminders);
        }

        [HttpGet("sort")]
        public IActionResult SortNotes(string sortBy = "creationDate")
        {
            IQueryable<Note> query = _context.Notes.Include(n => n.Tags);

            switch (sortBy.ToLower())
            {
                case "creationdate":
                    query = query.OrderBy(n => n.CreationDate);
                    break;
               

                default:
                    return BadRequest("Invalid sorting parameter");
            }

            var notes = query.ToList();
            return Ok(notes);
        }

        [HttpGet("search-filter-sort")]
        public IActionResult SearchFilterSort(string query, string tagName, string sortBy = "creationDate")
        {
            IQueryable<Note> queryable = _context.Notes.Include(n => n.Tags);

           
            if (!string.IsNullOrEmpty(query))
            {
                queryable = queryable.Where(n => n.Title.Contains(query) || n.Content.Contains(query));
            }

           
            if (!string.IsNullOrEmpty(tagName))
            {
                queryable = queryable.Where(n => n.Tags.Any(t => t.Name == tagName));
            }

           
            switch (sortBy.ToLower())
            {
                case "creationdate":
                    queryable = queryable.OrderBy(n => n.CreationDate);
                    break;
               

                default:
                    return BadRequest("Invalid sorting parameter");
            }

            var result = queryable.ToList();
            return Ok(result);
        }

        // Получение заметки по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var note = await _context.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.NoteId == id);

            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }

        [HttpGet("send")]
        public async Task<IActionResult> SendReminders([FromQuery] int noteId)
        {
            var note = await _context.Notes
                .Include(n => n.Reminder)
                .FirstOrDefaultAsync(n => n.NoteId == noteId && n.ReminderDate.HasValue);

            if (note == null || note.Reminder == null)
            {
                return NotFound("Заметка или напоминание не найдены");
            }

            var message = $"Напоминание для заметки \"{note.Title}\": {note.Reminder.ReminderText}";

            await _notificationService.SendReminders(noteId, message);

            return Ok("Уведомление отправлено успешно.");
        }

     
        // Создание новой заметки
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] Note model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Проверяем существующие теги и создаем их при необходимости
                foreach (var tag in model.Tags)
                {
                    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag.Name);

                    if (existingTag == null)
                    {
                        _context.Tags.Add(tag);
                    }
                    else
                    {
                        tag.TagId = existingTag.TagId;
                    }
                }

                // Добавляем заметку и ее теги
                model.Tags.ForEach(tag => model.Tags.Add(tag));
                _context.Notes.Add(model);

                // Если указана дата напоминания, создаем напоминание
                if (model.ReminderDate.HasValue)
                {
                    model.Reminder = new Reminder
                    {
                        ReminderDate = model.ReminderDate.Value
                    };
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNoteById), new { id = model.NoteId }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        // PUT api/notes/updatenotewithtags/{id}
        [HttpPut("updatenotewithtags/{id}")]
        public async Task<IActionResult> ОбновитьЗаметкуСТегами(int id, [FromBody] NoteWithTags updatedNote)
        {
            try
            {
                var existingNote = await _context.Notes
                    .Include(n => n.Tags) 
                    .FirstOrDefaultAsync(n => n.NoteId == id);

                if (existingNote == null)
                {
                    return NotFound("Заметка не найдена.");
                }

            
                existingNote.Title = updatedNote.Title;
                existingNote.Content = updatedNote.Content;
                existingNote.ReminderDate = updatedNote.ReminderDate;

             
                existingNote.Tags = updatedNote.Tags;

                await _context.SaveChangesAsync();

                return Ok("Заметка успешно обновлена.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // POST api/notes/assigntag
        [HttpPost("assigntag")]
        public async Task<IActionResult> AssignTagToNote([FromBody] NoteTagDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

              
                var note = await _context.Notes.FindAsync(model.NoteID);
                var tag = await _context.Tags.FindAsync(model.TagID);

                if (note == null || tag == null)
                {
                    return NotFound("Заметка или тег не найдены.");
                }

             
                if (!note.Tags.Any(t => t.TagId == model.TagID))
                {
               
                    note.Tags.Add(tag);
                    await _context.SaveChangesAsync();

                
                    _context.Entry(note).Reload();
                }

                return NoContent(); // Вернуть статус кода 204 (No Content)
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }



        [HttpPost("send")]
        public async Task<IActionResult> SendReminders([FromBody] ReminderRequestModel model)
        {
            int noteId = model.NoteId;

            var note = await _context.Notes
                .Include(n => n.Reminder)
                .FirstOrDefaultAsync(n => n.NoteId == noteId);

            if (note == null || note.Reminder == null)
            {
                return NotFound("Заметка или напоминание не найдены");
            }

            var message = $"Напоминание для заметки \"{note.Title}\": {model.Message}";

            await _notificationService.SendReminders(noteId, message);

            return Ok("Уведомление отправлено успешно.");
        }

        [HttpPost("{noteId}/set-reminder")]
        public async Task<IActionResult> SetReminder(int noteId, [FromBody] ReminderRequestModel model)
        {
            var note = await _context.Notes.FindAsync(noteId);

            if (note == null)
            {
                return NotFound("Заметка не найдена");
            }

            note.ReminderDate = model.ReminderDate;
            await _context.SaveChangesAsync();

            return Ok($"Напоминание для заметки \"{note.Title}\" установлено на {model.ReminderDate}");
        }



        // Редактирование заметки
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Note updatedNote)
        {
            try
            {
                if (updatedNote == null || id != updatedNote.NoteId)
                {
                    return BadRequest("Invalid data");
                }

                // Получение существующей заметки из базы данных
                var existingNote = await _context.Notes
                    .Include(n => n.Reminder)
                    .FirstOrDefaultAsync(n => n.NoteId == id);

                if (existingNote == null)
                {
                    return NotFound();
                }

                // Обновление свойств заметки
                existingNote.Title = updatedNote.Title;
                existingNote.Content = updatedNote.Content;
                existingNote.ReminderDate = updatedNote.ReminderDate; 

                // Если указана дата напоминания, обновляем или создаем напоминание
                if (existingNote.ReminderDate.HasValue)
                {
                    existingNote.Reminder ??= new Reminder();

                    existingNote.Reminder.ReminderDate = existingNote.ReminderDate.Value;
                }
                else
                {
                    existingNote.Reminder = null; // Если дата напоминания не указана, удаляем напоминание
                }

             
                await _context.SaveChangesAsync();

                // Возвращаем обновленный объект заметки
                return Ok(existingNote);
            }
            catch (Exception ex)
            {
                return Problem($"Error: An error occurred while saving the entity changes. See the inner exception for details. Inner Exception: {ex.Message}");
            }
        }


        // Удаление заметки
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                var note = await _context.Notes
                    .Include(n => n.Reminder) 
                    .FirstOrDefaultAsync(n => n.NoteId == id);

                if (note == null)
                {
                    return NotFound();
                }

              
                if (note.Reminder != null)
                {
                    _context.Reminders.Remove(note.Reminder);
                }

                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }
    }
}