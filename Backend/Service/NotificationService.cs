/*using Microsoft.AspNetCore.SignalR;

namespace NotesApp_Postgre.Service
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToClientAsync(int noteId, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", noteId, message);
        }
    }
}
*/