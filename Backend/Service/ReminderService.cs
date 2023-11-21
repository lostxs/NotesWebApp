using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotesApp_Postgre.Context;
using NotesApp_Postgre.Models;

namespace NotesApp_Postgre.Service
{
    public class ReminderService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReminderService(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        //SendReminders
        public async Task SendReminders(int noteId, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", noteId, message);
        }

        public async Task SaveReminder(Reminder reminder)
        {
            reminder.ReminderDate = reminder.ReminderDate.ToUniversalTime();
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }


        public async Task<Reminder> AddReminder(Reminder newReminder)
        {
            _context.Reminders.Add(newReminder);
            await _context.SaveChangesAsync();
            return newReminder;
        }

        public async Task<List<Reminder>> GetAllReminders()
        {
            return await _context.Reminders.ToListAsync();
        }
    }
}
