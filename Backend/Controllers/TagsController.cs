using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Context;
using NotesApp_Postgre.Models;

namespace NotesApp_Postgre.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        // Получение списка всех тегов
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags);
        }

        // Получение тега по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagId == id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        // Создание нового тега CreateTag
        [HttpPost]
        public IActionResult CreateTag([FromBody] Tag tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag.Name))
            {
                return BadRequest("Invalid tag data");
            }

            var existingTag = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);

            if (existingTag != null)
            {
                return Conflict("Tag with the same name already exists");
            }

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTags), tag);
        }



        // Удаление тега
        [HttpDelete("{id}")]
        public IActionResult DeleteTag(int id)
        {
            var tag = _context.Tags.Find(id);

            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
